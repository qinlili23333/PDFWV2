﻿using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Text;

namespace PDFWV2.PDFEngines
{
    internal class AdobeController : PDFEngineController
    {
        private Stream? DocumentStream;
        private string DocumentPath = string.Empty;
        private string FileName = "DefaultFileName.pdf";
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
            if (Path.StartsWith("http"))
            {
                // Online file
                PrepareURL(Path);
            }
            else
            {
                // Local file
                DocumentPath = Path;
                PreloadMode = true;
                FileName = System.IO.Path.GetFileName(DocumentPath);
                PrepareFileStream();
            }
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

        /// <summary>
        /// Read file to memory stream
        /// </summary>
        internal void PrepareFileStream()
        {
            DocumentStream = File.OpenRead(DocumentPath);
            FulfillStream(DocumentStream);
        }

        /// <summary>
        /// Load file only when page initialized
        /// </summary>
        internal async void PrepareURL(string url)
        {
            await WaitReady();
            PDFWindow.WebView.CoreWebView2.NavigationCompleted += delegate (
                object? sender, CoreWebView2NavigationCompletedEventArgs args)
            {
                PDFWindow.WebView.CoreWebView2.ExecuteScriptAsync($"FulfillURL(\"{url}\",\"{FileName}\");");
            };
        }

        /// <summary>
        /// Wait until WebView initialized
        /// </summary>
        /// <returns></returns>
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
      "*", CoreWebView2WebResourceContext.XmlHttpRequest);
            CoreWebView2WebResourceResponse PDFResp = PDFWindow.WebView.CoreWebView2.Environment.CreateWebResourceResponse(Stream, 200, "OK", "Content-Type: application/pdf");
            PDFWindow.WebView.CoreWebView2.WebResourceRequested += delegate (
               object? sender, CoreWebView2WebResourceRequestedEventArgs args)
            {
                // Adobe PDF has nothing to save on same PDF, so no need for hashed URL to enable progress saving
                if (args.Request.Uri == $"https://{PDFWV2InstanceManager.Options.LocalDomain}/Stream.pdf")
                {
                    args.Response = PDFResp;
                }
            };
            // TODO: Add API to show file name
            PDFWindow.WebView.CoreWebView2.NavigationCompleted += delegate (
                object? sender, CoreWebView2NavigationCompletedEventArgs args)
            {
                PDFWindow.WebView.CoreWebView2.ExecuteScriptAsync($"FulfillURL(\"https://{PDFWV2InstanceManager.Options.LocalDomain}/Stream.pdf\",\"{FileName}\");");
            };
        }

        internal override void OnWebViewReady(PDFWindow Window)
        {
            PDFWindow = Window;
            PDFWindow.WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync($"const AdobeKey=\"{PDFWV2InstanceManager.Options.AdobeKey}\";");
            PDFWindow.WebView.CoreWebView2.AddWebResourceRequestedFilter(
      "*", CoreWebView2WebResourceContext.Document);
            byte[] byteArray = Encoding.ASCII.GetBytes(WebRes.WebRes.AdobeWeb);
            MemoryStream stream = new(byteArray);
            CoreWebView2WebResourceResponse WebResp = PDFWindow.WebView.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200, "OK", "Content-Type: text/html");
            PDFWindow.WebView.CoreWebView2.WebResourceRequested += delegate (
   object? sender, CoreWebView2WebResourceRequestedEventArgs args)
            {
                if (args.Request.Uri == $"https://{PDFWV2InstanceManager.Options.LocalDomain}/Adobe.html")
                {
                    args.Response = WebResp;
                }
            };
            Window.WebView.CoreWebView2.Navigate($"https://{PDFWV2InstanceManager.Options.LocalDomain}/Adobe.html");
            Initialized = true;
            InitializeTCS.TrySetResult(true);
        }

        internal override void Dispose()
        {
            DocumentStream?.Dispose();
        }
    }
}
