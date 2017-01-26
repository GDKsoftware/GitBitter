namespace GitBitterLib
{
    using System;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using System.IO;
    using LibGit2Sharp.Handlers;

    public class GitSharpCloner : ICloner
    {
        private const string appNameBitbucket = "gitbitter:bitbucket";
        private const string appNameGithub = "gitbitter:github";
        private Identity identity;

        public GitSharpCloner()
        {
            var gitConfig = new GitConfig();
            identity = new Identity(gitConfig.UserName, gitConfig.UserEmail);
        }

        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            return CloneBitBucketRepo(repository, rootdir, repodir, branch);
        }

        private CredentialsHandler GetCredentialHandler(string repository)
        {
            var credentials = new SecureUsernamePasswordCredentials();
            if (repository.Contains("bitbucket"))
            {
                var cred = CredentialManager.ReadCredential(appNameBitbucket);
                credentials.Username = cred.UserName;
                credentials.Password = cred.Password;
            }
            else if (repository.Contains("github"))
            {
                var cred = CredentialManager.ReadCredential(appNameGithub);
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

            options.CredentialsProvider = GetCredentialHandler(repository);

            return options;
        }

        private Task CloneBitBucketRepo(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var options = GetCloneOptions(repository, branch);
                var fullRepoPath = Path.Combine(rootdir, repodir);

                Repository.Clone(repository, fullRepoPath, options);
            });
            task.Start();
            return task;
        }

        public Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var fullRepoPath = Path.Combine(rootdir, repodir);

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
