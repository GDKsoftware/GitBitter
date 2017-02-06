namespace GitBitterEdit
{
    using System;
    using GitBitterLib;

    public class CredentialUIMono : ICredentialUI
    {
        public CredentialUIMono()
        {
        }

        public PromptCredentials PromptForCredentialsWithSecureString(string targetName, string caption, string message)
        {
            var form = new LoginForm();
            form.TargetName = targetName;
            form.ShowDialog();

            return form.Credentials;
        }
    }
}
