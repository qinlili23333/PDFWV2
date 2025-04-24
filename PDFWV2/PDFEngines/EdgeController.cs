using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PDFWV2.PDFEngines
{
    internal class EdgeController : PDFEngineController
    {
        private Stream? DocumentStream;
        private string DocumentPath = string.Empty;
        private bool PreloadMode = false;
        private PDFWindow? PDFWindow;

        private bool Initialized = false;
        private TaskCompletionSource<bool> InitializeTCS = new();

        /// <summary>
        /// Create with ready stream
        /// </summary>
        /// <param name="Stream">Stream, prefer MemoryStream</param>
        internal EdgeController(Stream Stream)
        {
            DocumentStream = Stream;
            LoadStream(Stream);
        }

        /// <summary>
        /// Create with URL path, can be local file URI or online link URL
        /// </summary>
        /// <param name="Path"></param>
        internal EdgeController(string Path)
        {
            DocumentPath = Path;
        }

        /// <summary>
        /// Create empty controller and fulfill content later
        /// </summary>
        internal EdgeController()
        {
            PreloadMode = true;
        }

        /// <summary>
        /// Fulfill stream after controller loaded
        /// </summary>
        /// <param name="Stream">File stream</param>
        internal void FulfillStream(Stream Stream)
        {
            if (PreloadMode)
            {
                DocumentStream = Stream;
                PreloadMode = false;
                LoadStream(Stream);
            }
            else
            {
                throw new InvalidOperationException("You cannot fulfill stream to a loaded controller!");
            }
        }

        private async Task WaitReady()
        {
            if (Initialized)
            {
                return;
            }
            else
            {
                await InitializeTCS.Task;
            }
        }

        /// <summary>
        /// Load stream to engine by rewriting response
        /// </summary>
        /// <param name="Stream">File stream</param>
        private async Task LoadStream(Stream Stream)
        {
            await WaitReady();
            PDFWindow.WebView.CoreWebView2.AddWebResourceRequestedFilter(
      "*", CoreWebView2WebResourceContext.Document);
            PDFWindow.WebView.CoreWebView2.WebResourceRequested += delegate (
               object? sender, CoreWebView2WebResourceRequestedEventArgs args)
            {
                // Edge PDF has nothing to save on same PDF, so no need for hashed URL to enable progress saving
                if(args.Request.Uri==$"https://{PDFWV2InstanceManager.LocalDomain}/Stream.pdf")
                { 
                   CoreWebView2WebResourceResponse response = PDFWindow.WebView.CoreWebView2.Environment.CreateWebResourceResponse(Stream, 200, "OK", "Content-Type: application/pdf");
                   args.Response = response;
                }
            };
            PDFWindow.WebView.CoreWebView2.Navigate($"https://{PDFWV2InstanceManager.LocalDomain}/Stream.pdf");
        }

        internal override void OnWebViewReady(PDFWindow Window)
        {
            PDFWindow = Window;
            Window.WebView.CoreWebView2.Settings.HiddenPdfToolbarItems = CoreWebView2PdfToolbarItems.Save | CoreWebView2PdfToolbarItems.FullScreen;
            Window.WebView.CoreWebView2.ContextMenuRequested += delegate (object? sender,
                                    CoreWebView2ContextMenuRequestedEventArgs args)
            {
                IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;
                for (int index = 0; index < menuList.Count;)
                {
                    string[] keywords = ["back", "forward", "reload", "saveAs", "other", "webCapture"];
                    if (keywords.Contains(menuList[index].Name))
                    {
                        menuList.Remove(menuList[index]);
                    }
                    else
                    {
                        index++;
                    }
                }
                // Print menu when right click document area does nothing
                // I have reported to Microsoft
                // https://github.com/MicrosoftEdge/WebView2Feedback/issues/5213
                // In short time, let's do some workaround, I mean just disable it
                if (menuList.Count == 1 && menuList[0].Name == "print")
                {
                    menuList.Clear();
                }
                return;
            };
            if (PreloadMode)
            {
                Window.WebView.CoreWebView2.NavigateToString(WebRes.WebRes.Loading);
            }
            if (DocumentPath != string.Empty)
            {
                Window.WebView.CoreWebView2.Navigate(DocumentPath);
            }
            Initialized=true;
            InitializeTCS.TrySetResult(true);
        }
    }
}
