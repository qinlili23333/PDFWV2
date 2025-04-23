using System.Diagnostics;
using System.Windows;

namespace PDFWV2
{
    /// <summary>
    /// Interaction logic for PDFWindow.xaml
    /// </summary>
    public partial class PDFWindow : Window
    {
        private PDFEngineController EngineController;

        internal PDFWindow(PDFEngineController Controller)
        {
            InitializeComponent();
            EngineController = Controller;
            Init();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PDFWV2Instance.AddWindow(this);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PDFWV2Instance.RemoveWindow(this);
        }

        private async Task Init()
        {
            await WebView.EnsureCoreWebView2Async(PDFWV2InstanceManager.WebView2Environment);
            WebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            WebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            WebView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            WebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            WebView.CoreWebView2.Settings.UserAgent += " qinlili23333-PDFWV2/" + typeof(PDFWV2Instance).Assembly.GetName().Version?.ToString();
            if (PDFWV2InstanceManager.Options?.DebugTool == false)
            {
                WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }
            WebView.CoreWebView2.NavigationStarting += (a, e) =>
            {
                if ((PDFWV2InstanceManager.Options?.NetworkRequestIsolation ?? false) && e.Uri.StartsWith("http") && !(new Uri(e.Uri).DnsSafeHost == PDFWV2InstanceManager.LocalDomain || e.Uri.StartsWith("data")))
                {
                    // Currently we just open in external browser
                    // TODO: if the link is another PDF, open in new window instead
                    e.Cancel = true;
                    ProcessStartInfo startInfo = new(e.Uri)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                }
            };
            EngineController.OnWebViewReady(this);
        }
    }
}
