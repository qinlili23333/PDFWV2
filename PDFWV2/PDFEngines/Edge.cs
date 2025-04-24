using PDFWV2.Utils;
using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class Edge : PDFEngine
    {
        public Edge() : base(string.Empty)
        {

        }
        /// <inheritdoc />
        public override UpdateResult Update()
        {
            // Edge PDF engine should always up to date.
            return UpdateResult.NoUpdate;
        }

        /// <inheritdoc />
        protected override PDFWindow ViewFileEngine(string Path)
        {
            // There is no need to convert to stream for Edge PDF
            // Just directly open it
            return new PDFWindow(new EdgeController(new System.Uri(Path).AbsoluteUri));
        }

        /// <inheritdoc />
        public override PDFWindow ViewStream(Stream Stream)
        {
            return new PDFWindow(new EdgeController(Stream));
        }

        /// <inheritdoc />
        public override PDFWindow ViewURL(string URL)
        {
            if (PDFWV2InstanceManager.Options.NetworkRequestIsolation)
            {
                var Controller = new EdgeController();
                GetFileAndFulfill(Controller, URL);
                return new PDFWindow(Controller);
            }
            else
            {
                return new PDFWindow(new EdgeController(URL));
            }
        }

        private async void GetFileAndFulfill(EdgeController Controller, string URL)
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
