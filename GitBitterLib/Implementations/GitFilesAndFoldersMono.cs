namespace GitBitterLib
{
    using System;
    using System.IO;
    
    public class GitFilesAndFoldersMono : IGitFilesAndFolders
    {
        private string homeFolder;

        public GitFilesAndFoldersMono()
        {
            LoadHomeFolder();
        }

        public string SSHPrivateKeyFile()
        {
            return Path.Combine(homeFolder, ".ssh/id_rsa");
        }

        public string SSHPublicKeyFile()
        {
            return Path.Combine(homeFolder, ".ssh/id_rsa.pub");
        }

        public string UserDotCredentials()
        {
            return Path.Combine(homeFolder, ".gitcredentials");
        }

        public string UserDotGitConfig()
        {
            return Path.Combine(homeFolder, ".gitconfig");
        }

        private void LoadHomeFolder()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                homeFolder = Environment.GetEnvironmentVariable("HOME");
            }
            else
            {
                homeFolder = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
        }
    }
}
