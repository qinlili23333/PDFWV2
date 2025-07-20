using Microsoft.Web.WebView2.Core;
using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class PDFJSController : PDFEngineController
    {
        private string FolderPath = string.Empty;
        private string Link = string.Empty;
        private string FilePath = string.Empty;
        private Stream? DocumentStream;
        private PDFJS Engine = (PDFJS)PDFWV2InstanceManager.ActiveEngines[Engines.PDFJS];

        internal PDFJSController(string Folder)
        {
            FolderPath = Folder;
        }

        internal PDFJSController(string Folder, string URL)
        {
            FolderPath = Folder;
            if (URL.StartsWith("http"))
            {
                Link = URL;
            }
            else
            {
                FilePath = URL;
            }
        }

        private PDFWindow? PDFWindow;

        internal override void Dispose()
        {
            DocumentStream?.Dispose();
        }

        internal override async void OnWebViewReady(PDFWindow Window)
        {
            PDFWindow = Window;
            if (!Engine.IsReady())
            {
                PDFWindow.WebView.CoreWebView2.NavigateToString(WebRes.WebRes.Loading);
                await Engine.ReadyTCS.Task;
            }
            await PDFWindow.WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(WebRes.WebRes.PDFJSHook);
            PDFWindow.WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(PDFWV2InstanceManager.Options.LocalDomain, FolderPath, CoreWebView2HostResourceAccessKind.Deny);
            if (Link != string.Empty)
            {
                PDFWindow.WebView.CoreWebView2.Navigate($"https://{PDFWV2InstanceManager.Options.LocalDomain}/web/viewer.html?file={Link}");
            }
            else if (FilePath != string.Empty)
            {
                DocumentStream = File.OpenRead(FilePath);
                CoreWebView2WebResourceResponse Resp = PDFWindow.WebView.CoreWebView2.Environment.CreateWebResourceResponse(DocumentStream, 200, "OK", "Content-Type: application/pdf");
                PDFWindow.WebView.CoreWebView2.AddWebResourceRequestedFilter(
      "https://pdf/Stream.pdf", CoreWebView2WebResourceContext.All);
                PDFWindow.WebView.CoreWebView2.WebResourceRequested += delegate (
                   object? sender, CoreWebView2WebResourceRequestedEventArgs args)
                {
                    // WebResourceRequested will not trigger with VirtualHostName, have to use different one
                    if (args.Request.Uri == $"https://pdf/Stream.pdf")
                    {
                        args.Response = Resp;
                    }
                };
                PDFWindow.WebView.CoreWebView2.Navigate($"https://{PDFWV2InstanceManager.Options.LocalDomain}/web/viewer.html?file=https://pdf/Stream.pdf");
            }
        }
    }
}
