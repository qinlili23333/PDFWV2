using System.Windows;
using PDFWV2;

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

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            //Put options for debug here currently
            //TODO: make it configurable through GUI
            PDFWV2Options Options = new PDFWV2Options { 
                DebugTool = false, 
                Engine=Engines.EDGE,
                NetworkRequestIsolation=false
            };
            PDFWV2Instance PDF = await PDFWV2Instance.CreateInstance(Options);
            PDFEngine Engine = await PDF.CreateEngine();
            if (Type.SelectedIndex == 0)
            {
                Engine.ViewFile(Path.Text).Show();
            }
            else
            {
                Engine.ViewURL(Path.Text).Show();

            }
                Close();
        }
    }
}
