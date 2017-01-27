namespace GitBitterLib
{
    using System.IO;
    using System.Threading;
    using Newtonsoft.Json;

    public class PackageConfig
    {
        public PackageSettings Settings = null;
        public string Filename = string.Empty;

        protected void LoadFromFile()
        {
            var file = File.OpenText(Filename);
            try
            {
                var serializer = new JsonSerializer();
                Settings = (PackageSettings)serializer.Deserialize(file, typeof(PackageSettings));
            }
            finally
            {
                file.Close();
            }
        }

        protected bool CanBeOpened()
        {
            try
            {
                var file = File.Open(Filename, FileMode.Open, FileAccess.Write);
                file.Close();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public PackageConfig(string packageSettingsFile)
        {
            if (!File.Exists(packageSettingsFile))
            {
                this.Filename = packageSettingsFile;

                this.Settings = new PackageSettings();
            }
            else
            {
                this.Filename = packageSettingsFile;

                LoadFromFile();
            }
        }

        public void MakeBackup()
        {
            if (File.Exists(Filename))
            {
                File.Copy(Filename, Filename + ".bak", true);
            }
        }

        public void Save()
        {
            MakeBackup();

            while (File.Exists(Filename) && !CanBeOpened())
            {
                Thread.Sleep(100);
            }

            var file = File.CreateText(Filename);
            try
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, Settings);
            }
            finally
            {
                file.Close();
            }
        }
    }
}
