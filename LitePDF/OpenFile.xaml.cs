using System.Windows;

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
