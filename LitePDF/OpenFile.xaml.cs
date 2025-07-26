using PDFWV2;
using System.Windows;

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

        private async Task CreateEngine()
        {
            //Put options for debug here currently
            //TODO: make it configurable through GUI
            Engines Engines;
            if (EngineSelect.SelectedIndex == 0)
            {
                Engines = Engines.EDGE;
            }
            else if (EngineSelect.SelectedIndex == 1)
            {
                Engines = Engines.PDFJS;
            }
            else
            {
                Engines = Engines.Adobe;
            }
            PDFWV2Options Options = new()
            {
                DebugTool = DevTool.IsChecked ?? false,
                DefaultEngine = Engines,
                NetworkRequestIsolation = NetworkRequestIsolation.IsChecked ?? false,
                SecurityHardenLevel = SecurityLevel.Enhanced
            };
            PDF = await PDFWV2Instance.GetInstance(Options);
            DevTool.IsEnabled = false;
            NetworkRequestIsolation.IsEnabled = false;
            Engine = await PDF.CreateEngine(Engines);
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (Engine == null)
            {
                await CreateEngine();
            }
            if (Type.SelectedIndex == 0)
            {
                Engine.ViewFile(Path.Text);
            }
            else
            {
                Engine.ViewURL(Path.Text);

            }

        }

        private async void Preheat_Click(object sender, RoutedEventArgs e)
        {
            await CreateEngine();
        }
    }
}
