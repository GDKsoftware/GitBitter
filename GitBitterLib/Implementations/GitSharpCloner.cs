namespace GitBitterLib
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
        private GitConfig gitConfig;

        public GitSharpCloner()
        {
            gitConfig = new GitConfig();

            LoadSettings();
        }

        private void LoadSettings()
        {
            identity = new Identity(gitConfig.UserName, gitConfig.UserEmail);
        }

        private string GetUrlForRepository(string repository)
        {
            if (gitConfig.UseSSH)
            {
#if MONO
                // todo: find out why we need to do this on OSX/mono
                return repository.Replace("https://", "git://git@");
#else
				return repository.Replace("https://", "ssh://git@");
#endif
            }
            else
            {
                return repository;
            }
        }

        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var logging = GitBitterContainer.Default.Resolve<IGitBitterLogging>();

                string url = GetUrlForRepository(repository);

                string stage = "initialization";
                try
                {
                    var options = GetCloneOptions(url, branch);
                    var fullRepoPath = Path.Combine(rootdir, repodir);

                    stage = "cloning";
                    Repository.Clone(url, fullRepoPath, options);

                    logging.Add("Finished cloning " + repodir, LoggingLevel.Info);
                }
                catch (Exception ex)
                {
                    throw new ClonerException(ex, repodir, url, stage);
                }
            });
            task.Start();
            return task;
        }

        public Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var logging = GitBitterContainer.Default.Resolve<IGitBitterLogging>();

                string url = GetUrlForRepository(repository);

                string stage = "initialization";
                try
                {
                    var fullRepoPath = Path.Combine(rootdir, repodir);

                    var options = new PullOptions();
                    options.FetchOptions = new FetchOptions();

                    if (gitConfig.UseSSH)
                    {
                        options.FetchOptions.CredentialsProvider = GetCredentialHandlerSSH(repository);
                    }
                    else
                    {
                        options.FetchOptions.CredentialsProvider = GetCredentialHandler(repository);
                    }

                    var sig = new Signature(identity, DateTime.Now);

                    var repo = new Repository(fullRepoPath);

                    if (gitConfig.UseResetHard)
                    {
                        stage = "reset";
                        repo.Reset(ResetMode.Hard);
                    }

                    stage = "pull";
                    Commands.Pull(repo, sig, options);

                    stage = "checkout " + branch;
                    Commands.Checkout(repo, branch);

                    logging.Add("Finished updating " + repodir, LoggingLevel.Info);
                }
                catch (Exception ex)
                {
                    throw new ClonerException(ex, repodir, url, stage);
                }
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

            if (gitConfig.UseSSH)
            {
                options.CredentialsProvider = GetCredentialHandlerSSH(repository);
            }
            else
            {
                options.CredentialsProvider = GetCredentialHandler(repository);
            }

            return options;
        }
    }
}
