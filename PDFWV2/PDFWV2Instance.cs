using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;

namespace PDFWV2
{
    public class PDFWV2Instance
    {
        Process? CurrentProcess;

        internal PDFWV2Instance()
        {
            if (PDFWV2InstanceManager.Instance != null)
            {
                throw new Exception("Double Initialization! You can only initialize one instance in one process.");
            }
            CurrentProcess = Process.GetCurrentProcess();
            Directory.CreateDirectory(PDFWV2InstanceManager.Options.ModuleFolder);
        }

        /// <summary>
        /// Create a PDFWV2 instance
        /// </summary>
        /// <returns>Instance</returns>
        public static async Task<PDFWV2Instance> CreateInstance()
        {
            PDFWV2InstanceManager.Instance = new PDFWV2Instance();
            await InitAppWebView(PDFWV2InstanceManager.Options.ModuleFolder);
            return PDFWV2InstanceManager.Instance;
        }

        /// <summary>
        /// Create a PDFWV2 instance with options
        /// </summary>
        /// <param name="Options">PDFWV2Options object</param>
        /// <returns>Instance</returns>
        public static async Task<PDFWV2Instance> CreateInstance(PDFWV2Options Options)
        {
            PDFWV2InstanceManager.Options = Options;
            return await CreateInstance();
        }

        /// <summary>
        /// Get PDFWV2 instance, if an instance already exists, return it, otherwise create a new instance, with options if options is not null
        /// </summary>
        /// <param name="Options">PDFWV2Options object, or null to use default options</param>
        /// <returns>Instance</returns>
        public static async Task<PDFWV2Instance> GetInstance(PDFWV2Options? Options)
        {
            if(PDFWV2InstanceManager.Instance == null)
            {
                if (Options == null)
                {
                    return await CreateInstance();
                }
                else
                {
                    return await CreateInstance(Options);
                }
            }
            else
            {
                return PDFWV2InstanceManager.Instance;
            }
        }

        /// <summary>
        /// Dispose the instance for current process.
        /// </summary>
        /// <param name="Force">Whether to force close all opened documents</param>
        /// <returns>Async bool of whether dispose is success</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static async Task<bool> Dispose(bool Force)
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
            if (Force)
            {
                foreach (PDFWindow doc in PDFWV2InstanceManager.ActiveDocuments)
                {
                    doc.Close();
                }
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

        public async Task<PDFEngine> CreateEngine()
        {
            switch (PDFWV2InstanceManager.Options.Engine)
            {
                case Engines.EDGE:
                default:
                    return new PDFEngines.Edge();
                case Engines.PDFJS:
                    return new PDFEngines.PDFJS(PDFWV2InstanceManager.Options.ModuleFolder);
            }
        }

        /// <summary>
        /// Initialize WebView2
        /// </summary>
        /// <param name="Path">Data folder</param>
        /// <returns>Async void</returns>
        private async static Task InitAppWebView(string Path)
        {
            if (PDFWV2InstanceManager.WebView2Environment != null)
            {
                return;
            }
            else
            {
                var WebviewArgu = "--disable-features=msSmartScreenProtection,ElasticOverscroll --enable-features=msWebView2EnableDraggableRegions --in-process-gpu --disable-web-security --no-sandbox --allow-file-access-from-files";
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

        /// <summary>
        /// Add document window to instance manager to record active documents
        /// </summary>
        /// <param name="window">PDFWindow object</param>
        internal static void AddWindow(PDFWindow window)
        {
            PDFWV2InstanceManager.ActiveDocuments.Add(window);
        }

        /// <summary>
        /// Remove document window from active documents list
        /// </summary>
        /// <param name="window">PDFWindow object</param>
        internal static void RemoveWindow(PDFWindow window)
        {
            PDFWV2InstanceManager.ActiveDocuments.Remove(window);
        }
    }
}
