namespace GitBitterLib
{
    using System.Collections.Generic;
    using Octokit;

    public class GitHubLister : IBitterRepositoryLister
    {
        private const string AppName = "gitbitter:github";
        private const string PreferedLinkProtocol = "https";
        private GitHubClient github;
        private string username;

        /// <summary>
        /// Currently just for setting up login credentials
        /// </summary>
        /// <returns></returns>
        protected bool Login()
        {
            var cred = CredentialManager.ReadCredential(AppName);
            while (cred == null)
            {
                var promptedcredentials = CredentialUI.PromptForCredentialsWithSecureString(AppName, "GitBitter", "Please enter your GitHub login credentials");
                if (promptedcredentials != null)
                {
                    CredentialManager.WriteCredential(AppName, promptedcredentials.UserName, promptedcredentials.Password);

                    cred = CredentialManager.ReadCredential(AppName);
                }
                else
                {
                    return false;
                }
            }

            username = cred.UserName;
            cred = null;

            return true;
        }

        public List<RepositoryDescription> GetRepositories(string team)
        {
            github = new GitHubClient(new ProductHeaderValue("GitBitter"));

            Login();

            var lstRepos = new List<RepositoryDescription>();

            var task = github.Repository.GetAllForOrg(team);
            task.Wait();

            var repos = task.Result;
            foreach (var repo in repos)
            {
                var desc = new RepositoryDescription();
                desc.Name = repo.Name;
                desc.Description = repo.Description;
                desc.DefaultBranch = repo.DefaultBranch;
                desc.URL = repo.CloneUrl;

                lstRepos.Add(desc);
            }

            return lstRepos;
        }
    }
}
