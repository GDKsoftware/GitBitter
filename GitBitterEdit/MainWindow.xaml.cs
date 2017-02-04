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
using Microsoft.Practices.Unity;
using GitBitterLib;
using System.IO;

namespace GitBitterEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string teamNameBitbucket = "GDK";
        private const string teamNameGitHub = "gdksoftware";
        private string settingsPath;
        private PackageConfig config;

        public MainWindow()
        {
            InitializeComponent();

            GitBitterContainer.Default.RegisterType<ICloner, GitSharpCloner>();
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerWindows>();
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileWindows>();
            GitBitterContainer.Default.RegisterType<ICredentialUI, CredentialUIWindows>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersWindows>();

            settingsPath = Environment.CurrentDirectory;

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                this.config = new PackageConfig(args[1]);
            }
            else
            {
                this.config = new PackageConfig(Path.Combine(settingsPath, "gitbitter.json"));
            }

            this.RefreshPackageSettings();
        }

        private void RefreshPackageSettings()
        {
            listBox.ItemsSource = from package in this.config.Settings.Packages
                                  select package.Folder + " (" + package.Branch + ") <" + package.Repository + ">";
        }
        
        private void btnUpdateAll_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            try
            {
                var cloner = new PackageUnwrapper(this.config.Filename);
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
                this.Cursor = Cursors.Arrow;
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddFromBitBucket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var select = new RepoSelect(new BitbucketLister(), teamNameBitbucket);
                select.Owner = this;
                select.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var res = select.ShowDialog();
                if (res == true)
                {
                    var repo = select.SelectRepository;
                    if (repo != null)
                    {
                        this.AddPackage(repo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
            }
        }

        private void AddPackage(RepositoryDescription repo)
        {
            var package = new Package();
            package.Repository = repo.URL;
            package.Folder = repo.Name;
            if (!string.IsNullOrEmpty(repo.DefaultBranch))
            {
                package.Branch = repo.DefaultBranch;
            }

            this.config.Settings.Packages.Add(package);

            this.config.Save();

            RefreshPackageSettings();
        }

        private void btnAddFromGitHub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var select = new RepoSelect(new GitHubLister(), teamNameGitHub);
                select.Owner = this;
                select.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var res = select.ShowDialog();
                if (res == true)
                {
                    var repo = select.SelectRepository;
                    if (repo != null)
                    {
                        this.AddPackage(repo);
                    }
                }
            }
            catch (NotImplementedException)
            {
                MessageBox.Show("Oops, something hasn't been implemented yet...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
            }
        }
    }
}
