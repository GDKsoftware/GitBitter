namespace GitBitterLib
{
    using System;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using System.IO;
    using System.Runtime.InteropServices;

    public class GitSharpCloner : ICloner
    {
        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var options = new CloneOptions();
                options.BranchName = branch;
                options.Checkout = true;

                var credentials = new UsernamePasswordCredentials();
                // todo
                credentials.Username = "username";
                credentials.Password = "password";

                options.CredentialsProvider = (_url, _user, _cred) => credentials;

                var fullRepoPath = Path.Combine(rootdir, repodir);
                Repository.Clone(repository, fullRepoPath, options);
            });
            task.Start();
            return task;
        }

        public Task ResetAndUpdateExisting(string rootdir, string repodir, string branch)
        {
            throw new NotImplementedException();
        }
    }
}
