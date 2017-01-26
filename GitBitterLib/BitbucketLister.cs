namespace GitBitterLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SharpBucket.V2;
    using System.Net;

    public class BitbucketLister : IBitterRepositoryLister
    {
        private const string appName = "gitbitter:bitbucket";
        private const string preferedLinkProtocol = "https";
        private SharpBucketV2 sharpBucket;
        private string username;

        protected bool Login()
        {
            var cred = CredentialManager.ReadCredential(appName);
            while (cred == null)
            {
                var promptedcredentials = CredentialUI.PromptForCredentialsWithSecureString(appName, "GitBitter", "Please enter your BitBucket login credentials");
                if (promptedcredentials != null)
                {
                    CredentialManager.WriteCredential(appName, promptedcredentials.UserName, promptedcredentials.Password);

                    cred = CredentialManager.ReadCredential(appName);
                }
                else
                {
                    return false;
                }
            }

            sharpBucket.BasicAuthentication(cred.UserName, cred.Password.ToInsecureString());
            username = cred.UserName;
            cred = null;

            return true;
        }

        private string GetPreferredLinkFromRepo(SharpBucket.V2.Pocos.Repository project)
        {
            string url = "";

            foreach (var link in project.links.clone)
            {
                if (link.name == preferedLinkProtocol)
                {
                    url = link.href.Replace(username + "@", "");
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

                var userRepos = repoEndPoint.ListRepositories(team);
                foreach (var project in userRepos)
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
