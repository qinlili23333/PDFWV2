namespace PDFWV2.PDFEngines
{
    internal class MuPDFController : PDFEngineController
    {

        private PDFWindow? PDFWindow;

        internal override void Dispose()
        {
            throw new NotImplementedException();
        }

        internal override void OnWebViewReady(PDFWindow Window)
        {
            PDFWindow = Window;
        }
    }
}
