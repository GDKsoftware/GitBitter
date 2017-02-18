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

#if MONO
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileMadMilkman>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersMono>();
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerPlainText>();
            GitBitterContainer.Default.RegisterType<ICredentialUI, CredentialUIMono>();
#else
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerWindows>();
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileWindows>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersWindows>();
            GitBitterContainer.Default.RegisterType<ICredentialUI, CredentialUIWindows>();
#endif

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
                        AddPackage(repo);
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
            if (!String.IsNullOrEmpty(repo.DefaultBranch))
            {
                package.Branch = repo.DefaultBranch;
            }

            config.Settings.Packages.Add(package);

            config.Save();

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
                        AddPackage(repo);
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

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var idx = listBox.SelectedIndex;
            if (idx != -1)
            {
                var package = config.Settings.Packages[idx];

                EditPackage(package);
            }
        }

        private void EditPackage(Package package)
        {
            var form = new EditPackageDetails();
            form.Owner = this;
            form.SetObject(package);
            if (form.ShowDialog() == true)
            {
                config.Save();

                RefreshPackageSettings();
            }
        }

        private void RemovePackage(Package package)
        {
            config.Settings.Packages.Remove(package);
            config.Save();

            RefreshPackageSettings();
        }

        private void btnGitConfig_Click(object sender, RoutedEventArgs e)
        {
            var frm = new GitConfigEdit();
            frm.Owner = this;
            frm.ShowDialog();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var idx = listBox.SelectedIndex;
            if (idx != -1)
            {
                var package = config.Settings.Packages[idx];

                RemovePackage(package);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var idx = listBox.SelectedIndex;
            if (idx != -1)
            {
                var package = config.Settings.Packages[idx];

                EditPackage(package);
            }
        }
    }
}
