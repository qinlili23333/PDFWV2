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
            if (DocumentPath != string.Empty)
            {
                Window.WebView.CoreWebView2.Navigate(DocumentPath);
            }
        }
    }
}
