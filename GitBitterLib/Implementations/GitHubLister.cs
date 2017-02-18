namespace GitBitterLib
{
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;
    using Octokit;

    public class GitHubLister : IBitterRepositoryLister
    {
        private const string AppName = "gitbitter:github";
        private const string PreferedLinkProtocol = "https";
        private GitHubClient github;

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

        /// <summary>
        /// Currently just for setting up login credentials
        /// </summary>
        /// <returns></returns>
        private bool Login()
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            var cred = credmanager.ReadCredential(AppName);
            while (cred == null)
            {
                ICredentialUI credUI = GitBitterContainer.Default.Resolve<ICredentialUI>();
                var promptedcredentials = credUI.PromptForCredentialsWithSecureString(AppName, "GitBitter", "Please enter your GitHub login credentials");
                if (promptedcredentials != null)
                {
                    credmanager.WriteCredential(AppName, promptedcredentials.UserName, promptedcredentials.Password);

                    cred = credmanager.ReadCredential(AppName);
                }
                else
                {
                    return false;
                }
            }

            cred = null;

            return true;
        }
    }
}
