namespace GitBitterLib
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using SharpBucket.V2;
    using Unity;

    public class BitbucketLister : IBitterRepositoryLister
    {
        private const string AppName = "gitbitter:bitbucket";
        private const string PreferedLinkProtocol = "https";
        private SharpBucketV2 sharpBucket;
        private string username;

        public BitbucketLister()
        {
            sharpBucket = new SharpBucketV2();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public List<RepositoryDescription> GetRepositories(string team)
        {
            var repositories = new List<RepositoryDescription>();

            if (Login())
            {
                var repoEndPoint = sharpBucket.RepositoriesEndPoint();

                var teamRepositories = repoEndPoint.ListRepositories(team);
                foreach (var project in teamRepositories)
                {
                    var listerProject = new RepositoryDescription();
                    listerProject.Name = project.name;
                    listerProject.Description = project.description;
                    listerProject.URL = GetPreferredLinkFromRepo(project);

                    repositories.Add(listerProject);
                }
            }

            return repositories;
        }

        public List<string> GetTeams()
        {
            var teamnames = new List<string>();

            if (Login())
            {
                var endpoint = new BitbucketTeamsEndPoint(sharpBucket);
                var teams = endpoint.GetUserTeamsWithContributorRole();
                foreach (var team in teams)
                {
                    if (!teamnames.Contains(team.username))
                      teamnames.Add(team.username);
                }
            }

            return teamnames;
        }

        private bool Login()
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            var cred = credmanager.ReadCredentialOrNull(AppName);
            while (cred == null)
            {
                ICredentialUI credUI = GitBitterContainer.Default.Resolve<ICredentialUI>();
                var promptedcredentials = credUI.PromptForCredentialsWithSecureString(AppName, "GitBitter", "Please enter your BitBucket login credentials");
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

            sharpBucket.BasicAuthentication(cred.UserName, cred.Password.ToInSecureString());
            username = cred.UserName;
            cred = null;

            return true;
        }

        private string GetPreferredLinkFromRepo(SharpBucket.V2.Pocos.Repository project)
        {
            string url = string.Empty;

            foreach (var link in project.links.clone)
            {
                if (link.name == PreferedLinkProtocol)
                {
                    url = link.href.Replace(username + "@", string.Empty);
                    break;
                }
                else
                {
                    url = link.href;
                }
            }

            return url;
        }

        public void ResetCredentials()
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            credmanager.ResetCredential(AppName);
        }
    }
}
