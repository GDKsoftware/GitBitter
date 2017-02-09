namespace GitBitterLib
{
    public interface ICredentialUI
    {
        PromptCredentials PromptForCredentialsWithSecureString(string targetName, string caption, string message);
    }
}
