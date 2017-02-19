namespace GitBitterLib
{
    using System.Collections.Generic;

    public interface IBitterRepositoryLister
    {
        List<RepositoryDescription> GetRepositories(string team);

        List<string> GetTeams();
    }

    public class RepositoryDescription
    {
        public string Name { get; set; }

        public string URL { get; set; }

        public string Description { get; set; }

        public string DefaultBranch { get; set; }
    }
}
