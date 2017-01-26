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
using System.Windows.Navigation;
using GitBitterLib;
using System.IO;

namespace GitBitterEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string settingsPath;
        private PackageConfig config;

        public MainWindow()
        {
            InitializeComponent();

            settingsPath = Environment.CurrentDirectory;

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                config = new PackageConfig(args[1]);
            }
            else
            {
                config = new PackageConfig(Path.Combine(settingsPath, "gitbitter.json"));
            }

            RefreshPackageSettings();
        }

        private void RefreshPackageSettings()
        {
            listBox.ItemsSource = from package in config.Settings.Packages
                                  select package.Folder + " (" + package.Branch + ") <" + package.Repository + ">";
        }
        
        private void btnUpdateAll_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                var cloner = new PackageUnwrapper(config.Filename);
                try
                {
                    cloner.StartAndWaitForUnwrapping();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAddFromBitBucket_Click(object sender, RoutedEventArgs e)
        {
            var select = new RepoSelect();
            select.Owner = this;
            select.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var res = select.ShowDialog();
            if (res == true)
            {
                var repo = select.SelectRepository;
                if (repo != null)
                {
                    AddPackage(repo);
                }
            }
        }

        private void AddPackage(RepositoryDescription repo)
        {
            var package = new Package();
            package.Repository = repo.URL;
            package.Folder = repo.Name;
            //package.Branch = repo.DefaultBranch;

            config.Settings.Packages.Add(package);

            config.Save();

            RefreshPackageSettings();
        }
    }
}
