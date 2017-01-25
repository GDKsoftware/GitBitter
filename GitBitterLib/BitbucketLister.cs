namespace GitBitterLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SharpBucket.V2;

    public class BitbucketLister : IPackageLister
    {
        public List<string> GetProjects(string team)
        {
            var projectNames = new List<string>();

            var sharpBucket = new SharpBucketV2();
            sharpBucket.OAuth2LeggedAuthentication("consumerkey", "secret");

            var repoEndPoint = sharpBucket.RepositoriesEndPoint();

            var userRepos = repoEndPoint.ListRepositories(team);
            foreach (var project in userRepos)
            {
                projectNames.Add(project.full_name);
            }

            return projectNames;
        }
    }
}
