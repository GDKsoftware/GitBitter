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

        public PackageUnwrapper(string APackageSettingsFile = "gitbitter.json") : base(APackageSettingsFile)
        {
            currentWorkdirectory = Path.GetDirectoryName(Path.GetFullPath(APackageSettingsFile));
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

        protected Task Unwrap(Package APackage)
        {
            var cloner = new GitSharpCloner();
            if (Directory.Exists(Path.Combine(currentWorkdirectory, APackage.Folder)))
            {
                return cloner.ResetAndUpdateExisting(APackage.Repository, currentWorkdirectory, APackage.Folder, APackage.Branch);
            }
            else
            {
                return cloner.Clone(APackage.Repository, currentWorkdirectory, APackage.Folder, APackage.Branch);
            }
        }
    }
}
