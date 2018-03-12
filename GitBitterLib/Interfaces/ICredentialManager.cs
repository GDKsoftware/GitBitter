namespace GitBitterLib
{
    using System;
    using System.Security;

    public class NoCredentialsSetForApplication: Exception
    {
        public NoCredentialsSetForApplication(string applicationName) : base("No credentials set for " + applicationName)
        {

        }
    }

    public interface ICredentialManager
    {
        Credential ReadCredential(string applicationName);

        Credential ReadCredentialOrNull(string applicationName);

        int WriteCredential(string applicationName, SecureString userName, SecureString password);

        void ResetCredential(string applicationName);
    }
}
