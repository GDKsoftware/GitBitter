namespace GitBitterLib
{
    using Newtonsoft.Json;
    using System.IO;

    public class Packages
    {
        protected PackageSettings settings;
        protected string packageSettingsFile;

        protected void LoadFromFile()
        {
            var file = File.OpenText(packageSettingsFile);
            var serializer = new JsonSerializer();
            settings = (PackageSettings)serializer.Deserialize(file, typeof(PackageSettings));
        }

        public Packages(string packageSettingsFile)
        {
            settings = new PackageSettings();
            this.packageSettingsFile = packageSettingsFile;

            LoadFromFile();
        }
    }

}
