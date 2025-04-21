using Microsoft.Web.WebView2.Core;
using System.IO;

namespace PDFWV2
{
    public class PDFWV2
    {
        internal PDFWV2(PDFWV2Options Options)
        {
            Directory.CreateDirectory(Options.ModuleFolder);
        }

        public async Task<PDFWV2> CreateInstance()
        {
            return await CreateInstance(new PDFWV2Options());
        }

        public async Task<PDFWV2> CreateInstance(PDFWV2Options Options)
        {
            if (PDFWV2InstanceManager.Instance != null)
            {
                throw new Exception("Double Initialization! You can only initialize one instance in one process.");
            }
            PDFWV2InstanceManager.Instance = new PDFWV2(Options);
            await InitAppWebView(Options.ModuleFolder);
            return PDFWV2InstanceManager.Instance;
        }

        public async Task<bool> Dispose(bool Force)
        {
            if (PDFWV2InstanceManager.Instance == null)
            {
                throw new NullReferenceException("Null Instance! You can only dispose an initialized instance.");
            }
            if (PDFWV2InstanceManager.ActiveDocuments.Count > 0 && !Force)
            {
                //Still has active documents
                return false;
            }
            var tcs = new TaskCompletionSource<bool>();
            PDFWV2InstanceManager.WebView2Environment.BrowserProcessExited += (o, e) =>
            {
                tcs.TrySetResult(true);
            };
            PDFWV2InstanceManager.AliveController.Close();
            await tcs.Task;
            return true;
        }
        private async static Task InitAppWebView(string Path)
        {
            if (PDFWV2InstanceManager.WebView2Environment != null)
            {
                return;
            }
            else
            {
                var WebviewArgu = "--disable-features=msSmartScreenProtection --enable-features=msWebView2EnableDraggableRegions --in-process-gpu --disable-web-security --no-sandbox";
                CoreWebView2EnvironmentOptions options = new()
                {
                    AdditionalBrowserArguments = WebviewArgu
                };
                Directory.CreateDirectory(Path + @"\WebviewCache\");
                PDFWV2InstanceManager.WebView2Environment = await CoreWebView2Environment.CreateAsync(null, Path + @"\WebviewCache\", options);
                //Create hidden WebView2 on MSG HWND to keep alive
                IntPtr HWND_MESSAGE = new(-3);
                PDFWV2InstanceManager.AliveController = await PDFWV2InstanceManager.WebView2Environment.CreateCoreWebView2ControllerAsync(HWND_MESSAGE);
            }
        }
    }
}
