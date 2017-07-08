namespace GitBitterLib
{
    using System.Security;

    public class PromptCredentials
    {
        public SecureString UserName { get; internal set; }

        public SecureString DomainName { get; internal set; }

        public SecureString Password { get; internal set; }

        public bool IsSaveChecked { get; set; }

        public PromptCredentials()
        {
        }

        public PromptCredentials(SecureString username, SecureString password)
        {
            this.UserName = username;
            this.Password = password;
        }
    }
}
