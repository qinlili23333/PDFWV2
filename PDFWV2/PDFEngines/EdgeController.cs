using Microsoft.Web.WebView2.Core;
using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class EdgeController : PDFEngineController
    {
        private Stream? DocumentStream;
        private string DocumentPath = string.Empty;
        internal EdgeController(Stream Stream)
        {
            DocumentStream = Stream;
        }
        internal EdgeController(string Path)
        {
            DocumentPath = Path;
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
                return;
            };
            if (DocumentPath != string.Empty)
            {
                Window.WebView.CoreWebView2.Navigate(DocumentPath);
            }
        }
    }
}
