namespace GitBitterLib
{
    using System;
    using System.IO;

    public class GitFilesAndFoldersWindows : IGitFilesAndFolders
    {
        public string UserDotCredentials()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            return Path.Combine(path, ".gitcredentials");
        }

        public string UserDotGitConfig()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            return Path.Combine(path, ".gitconfig");
        }
    }
}
