namespace GitBitterLib
{
    using System;
    using System.IO;

    public class GitConfig
    {
        public string UserName;
        public string UserEmail;

        protected string Filepath;

        public GitConfig()
        {
            DetermineFilepath();
            LoadConfigFile();
        }

        protected void LoadConfigFile()
        {
            var ini = new IniFile(Filepath);
            UserName = ini.IniReadValue("user", "name");
            UserEmail = ini.IniReadValue("user", "email");
        }

        protected void DetermineFilepath()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            Filepath = Path.Combine(path, ".gitconfig");
        }
    }
}
