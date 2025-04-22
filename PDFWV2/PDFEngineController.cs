namespace PDFWV2
{
    internal abstract class PDFEngineController
    {
        internal PDFEngineController()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call when PDFWindow successfully created WebView2Controller
        /// </summary>
        internal abstract void OnWebViewReady(PDFWindow Window);
    }
}
