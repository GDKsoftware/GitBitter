﻿namespace GitBitterLib
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using LibGit2Sharp.Handlers;
    using Microsoft.Practices.Unity;

    public class GitSharpCloner : ICloner
    {
        private const string AppNameBitbucket = "gitbitter:bitbucket";
        private const string AppNameGithub = "gitbitter:github";
        private Identity identity;
        private bool useSSH = false;

        public GitSharpCloner()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            var gitConfig = new GitConfig();

            identity = new Identity(gitConfig.UserName, gitConfig.UserEmail);
            useSSH = gitConfig.UseSSH;
        }

        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            return CloneBitBucketRepo(repository, rootdir, repodir, branch);
        }

        public Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var fullRepoPath = Path.Combine(rootdir, repodir);

                var options = new PullOptions();
                options.FetchOptions = new FetchOptions();

                if (useSSH)
                {
                    options.FetchOptions.CredentialsProvider = GetCredentialHandlerSSH(repository);
                }
                else
                {
                    options.FetchOptions.CredentialsProvider = GetCredentialHandler(repository);
                }

                var sig = new Signature(identity, DateTime.Now);

                var repo = new Repository(fullRepoPath);
                repo.Reset(ResetMode.Hard);

                Commands.Pull(repo, sig, options);

                Commands.Checkout(repo, branch);
            });
            task.Start();
            return task;
        }

        private CredentialsHandler GetCredentialHandlerSSH(string repository)
        {
            var filesAndFolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();

            var credentials = new SshUserKeyCredentials();
            credentials.Username = "git";
            credentials.Passphrase = string.Empty;
            credentials.PrivateKey = filesAndFolders.SSHPrivateKeyFile();
            credentials.PublicKey = filesAndFolders.SSHPublicKeyFile();

            if (!File.Exists(credentials.PrivateKey))
            {
                throw new FileNotFoundException("Private key not found @ " + credentials.PrivateKey);
            }

            if (!File.Exists(credentials.PublicKey))
            {
                throw new FileNotFoundException("Public key not found @ " + credentials.PublicKey);
            }

            return (_url, _user, _cred) => credentials;
        }

        private CredentialsHandler GetCredentialHandler(string repository)
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            var credentials = new SecureUsernamePasswordCredentials();
            if (repository.Contains("bitbucket"))
            {
                var cred = credmanager.ReadCredential(AppNameBitbucket);
                credentials.Username = cred.UserName;
                credentials.Password = cred.Password;
            }
            else if (repository.Contains("github"))
            {
                var cred = credmanager.ReadCredential(AppNameGithub);
                credentials.Username = cred.UserName;
                credentials.Password = cred.Password;
            }

            return (_url, _user, _cred) => credentials;
        }

        private CloneOptions GetCloneOptions(string repository, string branch)
        {
            var options = new CloneOptions();
            options.BranchName = branch;
            options.Checkout = true;

            if (useSSH)
            {
                options.CredentialsProvider = GetCredentialHandlerSSH(repository);
            }
            else
            {
                options.CredentialsProvider = GetCredentialHandler(repository);
            }

            return options;
        }

        private Task CloneBitBucketRepo(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                string url = repository;
                if (useSSH)
                {
                    url = repository.Replace("https://", "ssh://git@");
                }

                var options = GetCloneOptions(url, branch);
                var fullRepoPath = Path.Combine(rootdir, repodir);

                Repository.Clone(url, fullRepoPath, options);
            });
            task.Start();
            return task;
        }
    }
}
