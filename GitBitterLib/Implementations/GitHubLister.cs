namespace GitBitterLib
{
    using System;
    using System.Collections.Generic;
    using Unity;
    using Octokit;

    public class GitHubLister : IBitterRepositoryLister
    {
        private const string AppName = "gitbitter:github";
        private const string PreferedLinkProtocol = "https";
        private const string ProductName = "GitBitter";
        private Credential githubcredentials;
        private GitHubClient github;

        public List<RepositoryDescription> GetRepositories(string team)
        {
            Initialize();

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

        public List<string> GetTeams()
        {
            Initialize();

            var lstTeams = new List<string>();

            var task = github.Organization.GetAllForUser(githubcredentials.UserName);
            task.Wait();

            var teams = task.Result;
            foreach (var team in teams)
            {
                lstTeams.Add(team.Login);
            }

            return lstTeams;
        }

        private void Initialize()
        {
            if (github == null)
            {
                github = new GitHubClient(new ProductHeaderValue(ProductName));
                Login();
            }
        }

        /// <summary>
        /// Currently just for setting up login credentials
        /// </summary>
        /// <returns></returns>
        private bool Login()
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            githubcredentials = credmanager.ReadCredentialOrNull(AppName);

            while (githubcredentials == null)
            {
                ICredentialUI credUI = GitBitterContainer.Default.Resolve<ICredentialUI>();
                var promptedcredentials = credUI.PromptForCredentialsWithSecureString(AppName, "GitBitter", "Please enter your GitHub login credentials");
                if (promptedcredentials != null)
                {
                    credmanager.WriteCredential(AppName, promptedcredentials.UserName, promptedcredentials.Password);

                    githubcredentials = credmanager.ReadCredential(AppName);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetCredentials()
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            credmanager.ResetCredential(AppName);
        }
    }
}
