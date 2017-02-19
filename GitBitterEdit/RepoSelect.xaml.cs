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

namespace GitBitterEdit
{
    /// <summary>
    /// Interaction logic for RepoSelect.xaml
    /// </summary>
    public partial class RepoSelect : Window
    {
        private IBitterRepositoryLister repoLister;

        private string teamName
        {
            get
            {
                return edTeam.Text;
            }
        }

        public RepositoryDescription SelectRepository
        {
            get
            {
                return (RepositoryDescription)lstRepositories.SelectedItem;
            }
        }

        public RepoSelect(IBitterRepositoryLister repoLister)
        {
            InitializeComponent();

            this.repoLister = repoLister;

            RefreshTeams();
            RefreshList(teamName);
        }

        private void RefreshTeams()
        {
            string currentSelection = edTeam.Text;

            edTeam.ItemsSource = this.repoLister.GetTeams();

            if (currentSelection != "")
            {
                var idx = edTeam.Items.IndexOf(currentSelection);
                if (idx != -1)
                {
                    edTeam.SelectedIndex = idx;
                }
            }
            else
            {
                edTeam.SelectedIndex = 0;
            }
        }

        private void RefreshList(string teamName)
        {
            lstRepositories.ItemsSource = this.repoLister.GetRepositories(teamName);
            lstRepositories.DisplayMemberPath = "Name";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void lstRepositories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
        }
        
        private void edTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                RefreshList((string)e.AddedItems[0]);
            }
        }
    }
}
