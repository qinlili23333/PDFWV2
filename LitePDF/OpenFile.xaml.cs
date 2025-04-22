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

namespace LitePDF
{
    /// <summary>
    /// Interaction logic for OpenFile.xaml
    /// </summary>
    public partial class OpenFile : Window
    {
        public OpenFile()
        {
            InitializeComponent();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "PDF",
                DefaultExt = ".PDF",
                Filter = "Portable Document Format (.pdf)|*.pdf"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                Path.Text = dialog.FileName;
            }
        }
    }
}
