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

        public GitSharpCloner()
        {
            var gitConfig = new GitConfig();
            identity = new Identity(gitConfig.UserName, gitConfig.UserEmail);
        }

        protected string GetIdRSAFilepath()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            return Path.Combine(path, ".ssh/id_rsa");
        }

        protected string GetPublicKeyFilepath()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            return Path.Combine(path, ".ssh/id_rsa.pub");
        }

        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            return CloneBitBucketRepo(repository, rootdir, repodir, branch);
        }

        private CredentialsHandler GetCredentialHandlerSSH(string repository)
        {
            var credentials = new SshUserKeyCredentials();
            credentials.Username = "git";
            credentials.Passphrase = "";
            credentials.PrivateKey = GetIdRSAFilepath();
            credentials.PublicKey = GetPublicKeyFilepath();

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

            options.CredentialsProvider = GetCredentialHandlerSSH(repository);

            return options;
        }

        private Task CloneBitBucketRepo(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var url = repository.Replace("https://", "ssh://git@");
                var options = GetCloneOptions(url, branch);
                var fullRepoPath = Path.Combine(rootdir, repodir);

                Repository.Clone(url, fullRepoPath, options);
            });
            task.Start();
            return task;
        }

        public Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var fullRepoPath = Path.Combine(rootdir, repodir);

                var url = repository.Replace("https://", "ssh://git@");

                var options = new PullOptions();
                options.FetchOptions = new FetchOptions();
                options.FetchOptions.CredentialsProvider = GetCredentialHandler(repository);

                var sig = new Signature(identity, DateTime.Now);

                var repo = new Repository(fullRepoPath);
                repo.Reset(ResetMode.Hard);

                Commands.Pull(repo, sig, options);

                Commands.Checkout(repo, branch);
            });
            task.Start();
            return task;
        }
    }
}
