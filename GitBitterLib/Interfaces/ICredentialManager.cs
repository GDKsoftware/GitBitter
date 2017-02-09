namespace GitBitterLib
{
    using System.Security;

    public interface ICredentialManager
    {
        Credential ReadCredential(string applicationName);

        int WriteCredential(string applicationName, SecureString userName, SecureString password);
    }
}
