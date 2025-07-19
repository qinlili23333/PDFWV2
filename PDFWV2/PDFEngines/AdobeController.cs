using Microsoft.Web.WebView2.Core;
using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class AdobeController : PDFEngineController
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
        internal AdobeController(Stream Stream)
        {
            DocumentStream = Stream;
            LoadStream(Stream);
        }

        /// <summary>
        /// Create with URL path, can be local file URI or online link URL
        /// </summary>
        /// <param name="Path"></param>
        internal AdobeController(string Path)
        {
            DocumentPath = Path;
        }

        /// <summary>
        /// Create empty controller and fulfill content later
        /// </summary>
        internal AdobeController()
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
                if (args.Request.Uri == $"https://{PDFWV2InstanceManager.Options.LocalDomain}/Stream.pdf")
                {
                    CoreWebView2WebResourceResponse response = PDFWindow.WebView.CoreWebView2.Environment.CreateWebResourceResponse(Stream, 200, "OK", "Content-Type: application/pdf");
                    args.Response = response;
                }
            };
            PDFWindow.WebView.CoreWebView2.Navigate($"https://{PDFWV2InstanceManager.Options.LocalDomain}/Stream.pdf");
        }

        internal override void OnWebViewReady(PDFWindow Window)
        {
            PDFWindow = Window;
        }

        internal override void Dispose()
        {
            DocumentStream?.Dispose();
        }
    }
}
