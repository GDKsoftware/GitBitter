namespace GitBitterLib
{
    using System.Security;

    public enum CredentialType
    {
        Generic = 1,
        DomainPassword,
        DomainCertificate,
        DomainVisiblePassword,
        GenericCertificate,
        DomainExtended,
        Maximum,
        MaximumEx = Maximum + 1000,
    }

    public class Credential
    {
        private readonly string applicationName;
        private readonly string userName;
        private readonly SecureString password;
        private readonly CredentialType credentialType;

        public Credential(CredentialType credentialType, string applicationName, string userName, SecureString password)
        {
            this.applicationName = applicationName;
            this.userName = userName;
            this.password = password;
            this.credentialType = credentialType;
        }

        public CredentialType CredentialType
        {
            get { return credentialType; }
        }

        public string ApplicationName
        {
            get { return applicationName; }
        }

        public string UserName
        {
            get { return userName; }
        }

        public SecureString Password
        {
            get { return password; }
        }

        public override string ToString()
        {
            return string.Format("CredentialType: {0}, ApplicationName: {1}, UserName: {2}, Password: {3}", CredentialType, ApplicationName, UserName, Password);
        }
    }
}
