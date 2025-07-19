using PDFWV2.Utils;
using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class Adobe : PDFEngine
    {
        public Adobe() : base(string.Empty)
        {
            // Preload SDK in keep alive webview
            PDFWV2InstanceManager.AliveController?.CoreWebView2.NavigateToString(WebRes.WebRes.AdobeWeb);
        }
        /// <inheritdoc />
        public override UpdateResult Update()
        {
            // Adobe PDF engine should always up to date.
            return UpdateResult.NoUpdate;
        }

        /// <inheritdoc />
        protected override PDFWindow ViewFileEngine(string Path)
        {
            // There is no need to convert to stream for Adobe PDF
            // Just directly open it
            return new PDFWindow(new AdobeController(Path));
        }

        /// <inheritdoc />
        public override PDFWindow ViewStream(Stream Stream)
        {
            return new PDFWindow(new AdobeController(Stream));
        }

        /// <inheritdoc />
        public override PDFWindow ViewURL(string URL)
        {
            if (PDFWV2InstanceManager.Options.NetworkRequestIsolation)
            {
                var Controller = new AdobeController();
                GetFileAndFulfill(Controller, URL);
                return new PDFWindow(Controller);
            }
            else
            {
                return new PDFWindow(new AdobeController(URL));
            }
        }

        private async void GetFileAndFulfill(AdobeController Controller, string URL)
        {
            Stream ContentStream = await Network.GetHttpStream(URL);
            Controller.FulfillStream(ContentStream);
        }

        /// <inheritdoc />
        public override bool IsReady()
        {
            return true;
        }
    }

}
