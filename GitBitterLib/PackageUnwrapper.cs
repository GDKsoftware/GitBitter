namespace GitBitterLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Unity;

    public class PackageUnwrapper : PackageConfig
    {
        protected string currentWorkdirectory;

        private ICloner cloner;

        public PackageUnwrapper(string packageSettingsFile = "gitbitter.json") : base(packageSettingsFile)
        {
            currentWorkdirectory = Path.GetDirectoryName(Path.GetFullPath(packageSettingsFile));
            currentWorkdirectory = Path.GetFullPath(Path.Combine(currentWorkdirectory, ".."));

            cloner = GitBitterContainer.Default.Resolve<ICloner>();
        }

        public void StartAndWaitForUnwrapping()
        {
            var unwrappers = new List<Task>();
            foreach (var package in Settings.Packages)
            {
                var task = Unwrap(package);
                if (task != null)
                {
                    if (task.Status == TaskStatus.Created)
                    {
                        task.Start();
                    }

                    unwrappers.Add(task);
                }
            }

            Task.WaitAll(unwrappers.ToArray());
        }

        protected Task Unwrap(Package package)
        {
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
