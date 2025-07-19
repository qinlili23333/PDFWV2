using Microsoft.Web.WebView2.Core;

namespace PDFWV2.PDFEngines
{
    internal class PDFJSController : PDFEngineController
    {
        private string FolderPath = string.Empty;
        private string Link = string.Empty;

        internal PDFJSController(string Folder)
        {
            FolderPath = Folder;
        }

        internal PDFJSController(string Folder, string URL)
        {
            FolderPath = Folder;
            Link = URL;
        }

        private PDFWindow? PDFWindow;

        internal override void Dispose()
        {
        }

        internal override void OnWebViewReady(PDFWindow Window)
        {
            PDFWindow = Window;
            PDFWindow.WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(WebRes.WebRes.PDFJSHook);
            PDFWindow.WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(PDFWV2InstanceManager.Options.LocalDomain, FolderPath, CoreWebView2HostResourceAccessKind.Deny);
            PDFWindow.WebView.CoreWebView2.Navigate($"https://{PDFWV2InstanceManager.Options.LocalDomain}/web/viewer.html?file={Link}");
        }
    }
}
