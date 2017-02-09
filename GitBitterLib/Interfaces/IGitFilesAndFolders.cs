namespace GitBitterLib
{
    public interface IGitFilesAndFolders
    {
        string UserDotGitConfig();

        string UserDotCredentials();

        string SSHPrivateKeyFile();

        string SSHPublicKeyFile();
    }
}
