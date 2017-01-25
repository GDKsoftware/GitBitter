namespace GitBitterLib
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
