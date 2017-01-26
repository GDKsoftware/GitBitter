﻿namespace GitBitterLib
{
    using System.Collections.Generic;

    public class Package
    {
        public string Folder { get; set; }
        public string Repository { get; set; }
        public string Branch { get; set; }

        public Package()
        {
            Branch = "master";
        }

        public void SetDefaultFolder()
        {
            if (Repository != null)
            {
                var idxSlash = Repository.LastIndexOf("/");
                var idxDotGit = Repository.LastIndexOf(".git");
                if ((idxSlash != -1) && (idxDotGit != -1))
                {
                    Folder = Repository.Substring(idxSlash + 1, idxDotGit - 1 - idxSlash);
                }
            }
        }
    }

    public class PackageSettings
    {
        public List<Package> Packages;

        public PackageSettings()
        {
            Packages = new List<Package>();
        }
    }
}
