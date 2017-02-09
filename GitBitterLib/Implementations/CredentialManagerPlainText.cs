namespace GitBitterLib
{
    using System.Security;
    using Microsoft.Practices.Unity;

    public class CredentialManagerPlainText : ICredentialManager
    {
        private string filepath;

        public CredentialManagerPlainText()
        {
            DetermineFilepath();
        }

        public Credential ReadCredential(string applicationName)
        {
            IIniFile ini = GitBitterContainer.Default.Resolve<IIniFile>();
            ini.SetFile(filepath);

            var username = ini.IniReadValue(applicationName, "username");
            var password = ini.IniReadValue(applicationName, "password");

            if ((username == "") && (password == ""))
            {
                return null;
            }
            else
            {
                Credential cred = new Credential(
                    CredentialType.Generic,
                    applicationName,
                    username,
                    password.InsecureToSecureString());

                return cred;
            }
        }

        public int WriteCredential(string applicationName, SecureString userName, SecureString password)
        {
            IIniFile ini = GitBitterContainer.Default.Resolve<IIniFile>();
            ini.SetFile(filepath);

            ini.IniWriteValue(applicationName, "username", userName.ToInSecureString());
            ini.IniWriteValue(applicationName, "password", password.ToInSecureString());

            return 0;
        }

        private void DetermineFilepath()
        {
            IGitFilesAndFolders filesandfolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();
            filepath = filesandfolders.UserDotCredentials();
        }
    }
}
