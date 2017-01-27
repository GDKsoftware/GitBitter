namespace GitBitterLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PackageUnwrapper : PackageConfig
    {
        protected string currentWorkdirectory;

        public PackageUnwrapper(string packageSettingsFile = "gitbitter.json") : base(packageSettingsFile)
        {
            currentWorkdirectory = Path.GetDirectoryName(Path.GetFullPath(packageSettingsFile));
            currentWorkdirectory = Path.GetFullPath(Path.Combine(currentWorkdirectory, ".."));
        }

        public void StartAndWaitForUnwrapping()
        {
            var unwrappers = new List<Task>();
            foreach (var package in Settings.Packages)
            {
                unwrappers.Add(Unwrap(package));
            }

            Task.WaitAll(unwrappers.ToArray());
        }

        protected Task Unwrap(Package package)
        {
            var cloner = new GitSharpCloner();
            if (Directory.Exists(Path.Combine(currentWorkdirectory, package.Folder)))
            {
                return cloner.ResetAndUpdateExisting(package.Repository, currentWorkdirectory, package.Folder, package.Branch);
            }
            else
            {
                return cloner.Clone(package.Repository, currentWorkdirectory, package.Folder, package.Branch);
            }
        }
    }
}
