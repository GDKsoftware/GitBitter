namespace GitBitterLib
{
    using Microsoft.Practices.Unity;

    public class GitConfig
    {
        private const string SectionGitBitter = "gitbitter";
        private const string KeyUseSSH = "usessh";
        private const string SectionUser = "user";

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public bool UseSSH { get; set; }

        public GitConfig()
        {
            LoadConfigFiles();
        }

        public void Save()
        {
            var filesandfolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();

            var iniCredentials = GitBitterContainer.Default.Resolve<IIniFile>();
            iniCredentials.SetFile(filesandfolders.UserDotCredentials());

            if (UseSSH)
            {
                iniCredentials.IniWriteValue(SectionGitBitter, KeyUseSSH, "true");
            }
            else
            {
                iniCredentials.IniWriteValue(SectionGitBitter, KeyUseSSH, "false");
            }
        }

        protected void LoadConfigFiles()
        {
            var filesandfolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();

            var iniGitConfig = GitBitterContainer.Default.Resolve<IIniFile>();
            iniGitConfig.SetFile(filesandfolders.UserDotGitConfig());
            UserName = iniGitConfig.IniReadValue(SectionUser, "name");
            UserEmail = iniGitConfig.IniReadValue(SectionUser, "email");

            var iniCredentials = GitBitterContainer.Default.Resolve<IIniFile>();
            iniCredentials.SetFile(filesandfolders.UserDotCredentials());
            UseSSH = iniCredentials.IniReadValue(SectionGitBitter, KeyUseSSH).Equals("true");
        }
    }
}
