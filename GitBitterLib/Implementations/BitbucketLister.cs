namespace GitBitterLib
{
    using System.Collections.Generic;
    using SharpBucket.V2;
    using Microsoft.Practices.Unity;

    public class BitbucketLister : IBitterRepositoryLister
    {
        private const string AppName = "gitbitter:bitbucket";
        private const string PreferedLinkProtocol = "https";
        private SharpBucketV2 sharpBucket;
        private string username;

        protected bool Login()
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            var cred = credmanager.ReadCredential(AppName);
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

        public List<RepositoryDescription> GetRepositories(string team)
        {
            var repositories = new List<RepositoryDescription>();

            sharpBucket = new SharpBucketV2();
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
    }
}
