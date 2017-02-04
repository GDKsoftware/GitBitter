namespace GitBitterLib
{
    using Microsoft.Practices.Unity;

    public class GitConfig
    {
        public string UserName;
        public string UserEmail;

        protected string filepath;

        public GitConfig()
        {
            DetermineFilepath();
            LoadConfigFile();
        }

        protected void LoadConfigFile()
        {
            var ini = GitBitterContainer.Default.Resolve<IIniFile>();
            ini.SetFile(filepath);
            UserName = ini.IniReadValue("user", "name");
            UserEmail = ini.IniReadValue("user", "email");
        }

        protected void DetermineFilepath()
        {
            var filesandfolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();
            filepath = filesandfolders.UserDotGitConfig();
        }
    }
}
