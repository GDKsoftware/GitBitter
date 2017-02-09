namespace GitBitterEdit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using GitBitterLib;
    using System.Security;

    public partial class LoginForm : Window
    {
        public PromptCredentials Credentials { get; set; }

        public string TargetName
        {
            get
            {
                return this.Title;
            }

            set
            {
                this.Title = value;
            }
        }

        public LoginForm()
        {
            this.Credentials = null;

            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Credentials = new PromptCredentials(SecureStringHelper.InsecureToSecureString(EdUsername.Text), EdPassword.SecurePassword);

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Credentials = null;

            this.DialogResult = true;
        }
    }
}
