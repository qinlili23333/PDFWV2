using System.Windows;
using PDFWV2;

namespace LitePDF
{
    /// <summary>
    /// Interaction logic for OpenFile.xaml
    /// </summary>
    public partial class OpenFile : Window
    {
        PDFWV2Instance PDF;

        PDFEngine Engine;

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
            Engines Engines;
            if(EngineSelect.SelectedIndex==0)
            {
                Engines = Engines.EDGE;
            }
            else
            {
                Engines = Engines.PDFJS;
            }
            PDFWV2Options Options = new()
            {
                DebugTool = DevTool.IsChecked ?? false,
                DefaultEngine = Engines,
                NetworkRequestIsolation = NetworkRequestIsolation.IsChecked ?? false
            };
            PDF = await PDFWV2Instance.GetInstance(Options);
            DevTool.IsEnabled = false;
            NetworkRequestIsolation.IsEnabled = false;
            Engine = await PDF.CreateEngine(Engines);
            if (Type.SelectedIndex == 0)
            {
                Engine.ViewFile(Path.Text).Show();
            }
            else
            {
                Engine.ViewURL(Path.Text).Show();

            }

        }
    }
}
