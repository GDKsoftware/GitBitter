namespace GitBitterEdit
{
    using System.Windows;
    using GitBitterLib;

    /// <summary>
    /// Interaction logic for GitConfig.xaml
    /// </summary>
    public partial class GitConfigEdit : Window
    {
        public GitConfigEdit()
        {
            InitializeComponent();

            Load();
        }

        private void Load()
        {
            var config = new GitConfig();
            EdName.Text = config.UserName;
            EdEmail.Text = config.UserEmail;

            EdName.IsEnabled = false;
            EdEmail.IsEnabled = false;

            ChkUseSSH.IsChecked = config.UseSSH;
        }

        private void Save()
        {
            var config = new GitConfig();
            config.UseSSH = ChkUseSSH.IsChecked.GetValueOrDefault();

            config.Save();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
