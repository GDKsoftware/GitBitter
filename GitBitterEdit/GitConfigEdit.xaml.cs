namespace GitBitterEdit
{
    using System.Windows;
    using GitBitterLib;

    /// <summary>
    /// Interaction logic for GitConfig.xaml
    /// </summary>
    public partial class GitConfigEdit : Window
    {
        private GitConfig config;

        public GitConfigEdit()
        {
            InitializeComponent();

            Load();
        }

        private void Load()
        {
            config = new GitConfig();
            DataContext = config;
        }

        private void Save()
        {
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
