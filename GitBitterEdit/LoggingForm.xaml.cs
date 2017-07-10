using System;
using System.Collections;
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

namespace GitBitterEdit
{
    /// <summary>
    /// Interaction logic for LoggingForm.xaml
    /// </summary>
    public partial class LoggingForm : Window
    {
        public LoggingForm()
        {
            InitializeComponent();
        }

        public void SetList(IEnumerable ASource)
        {
            listBox.ItemsSource = null;
            listBox.ItemsSource = ASource;
        }
    }
}
