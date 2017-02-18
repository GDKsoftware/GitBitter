namespace GitBitterLib
{
    using System;
    using System.IO;
    using Microsoft.Practices.Unity;

    public class GitConfig
    {
        private const string SectionGitBitter = "gitbitter";
        private const string KeyUseSSH = "usessh";
        private const string KeyUseResetHard = "useresethard";
        private const string SectionUser = "user";
        private IGitFilesAndFolders filesAndFolders;

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public bool UseSSH { get; set; }

        public bool UseResetHard { get; set; }

        public GitConfig()
        {
            filesAndFolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();

            Load();
        }

        public void Save()
        {
            SaveUserDotConfig();
            SaveUserDotCredentials();
        }

        private void Load()
        {
            LoadUserDotConfig();
            LoadUserDotCredentials();
        }

        private void LoadUserDotConfig()
        {
            var iniGitConfig = GitBitterContainer.Default.Resolve<IIniFile>();
            iniGitConfig.SetFile(filesAndFolders.UserDotGitConfig());
            UserName = iniGitConfig.IniReadValue(SectionUser, "name");
            UserEmail = iniGitConfig.IniReadValue(SectionUser, "email");
        }

        private void LoadUserDotCredentials()
        {
            if (!File.Exists(filesAndFolders.UserDotCredentials()))
            {
                CreateEmptyUserDotCredentials();
            }

            var iniCredentials = GitBitterContainer.Default.Resolve<IIniFile>();
            iniCredentials.SetFile(filesAndFolders.UserDotCredentials());

            UseSSH = iniCredentials.IniReadValue(SectionGitBitter, KeyUseSSH).Equals("true");
            UseResetHard = !iniCredentials.IniReadValue(SectionGitBitter, KeyUseResetHard).Equals("false");
        }

        private void SaveUserDotConfig()
        {
            var iniGitConfig = GitBitterContainer.Default.Resolve<IIniFile>();
            iniGitConfig.SetFile(filesAndFolders.UserDotGitConfig());
            iniGitConfig.IniWriteValue(SectionUser, "name", UserName);
            iniGitConfig.IniWriteValue(SectionUser, "email", UserEmail);
        }

        private string Bool2String(bool value)
        {
            if (value)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        private void SaveUserDotCredentials()
        {
            var iniCredentials = GitBitterContainer.Default.Resolve<IIniFile>();
            iniCredentials.SetFile(filesAndFolders.UserDotCredentials());

            iniCredentials.IniWriteValue(SectionGitBitter, KeyUseSSH, Bool2String(UseSSH));
            iniCredentials.IniWriteValue(SectionGitBitter, KeyUseResetHard, Bool2String(UseResetHard));
        }

        private void InitializeUserDotCredentialsDefaults()
        {
            UseSSH = true;
            UseResetHard = true;
        }

        private void CreateEmptyUserDotCredentials()
        {
            InitializeUserDotCredentialsDefaults();
            Save();
        }
    }
}
