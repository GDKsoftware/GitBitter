namespace GitBitterEdit
{
    using System.Windows;
    using GitBitterLib;

    /// <summary>
    /// Interaction logic for EditDetails.xaml
    /// </summary>
    public partial class EditPackageDetails : Window
    {
        private Package package = null;
        private Package packageCopy = null;

        public EditPackageDetails()
        {
            InitializeComponent();
        }

        public void SetObject(Package package)
        {
            this.package = package;
            packageCopy = (Package)package.Clone();

            DataContext = packageCopy;
        }

        protected void Save()
        {
            package.Folder = packageCopy.Folder;
            package.Repository = packageCopy.Repository;
            package.Branch = packageCopy.Branch;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Save();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
