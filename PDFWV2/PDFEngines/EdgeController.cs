using Microsoft.Web.WebView2.Core;
using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class EdgeController : PDFEngineController
    {
        private Stream? DocumentStream;
        private string DocumentPath = string.Empty;
        private bool PreloadMode = false;

        /// <summary>
        /// Create with ready stream
        /// </summary>
        /// <param name="Stream">Stream, prefer MemoryStream</param>
        internal EdgeController(Stream Stream)
        {
            DocumentStream = Stream;
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
        /// 
        /// </summary>
        /// <param name="Stream"></param>
        internal void FulfillStream(Stream Stream)
        {
            if (PreloadMode)
            {
                DocumentStream = Stream;
                PreloadMode = false;
                //TODO: load stream
            }
            else
            {
                throw new InvalidOperationException("You cannot fulfill stream to a loaded controller!");
            }
        }

        internal override void OnWebViewReady(PDFWindow Window)
        {
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
        }
    }
}
