namespace GitBitterLib
{
    using System.Collections.Generic;

    public interface IPackageLister
    {
        List<string> GetProjects(string team);
    }
}
