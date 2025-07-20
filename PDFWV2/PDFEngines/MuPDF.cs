using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class MuPDF : PDFEngine
    {
        private string FolderPath = string.Empty;

        public MuPDF(string ModuleFolder) : base(ModuleFolder)
        {
            FolderPath = ModuleFolder + "\\MuPDF";
        }
        public override bool IsReady()
        {
            throw new NotImplementedException();
        }

        public override Task<UpdateResult> Update()
        {
            throw new NotImplementedException();
        }

        public override PDFWindow ViewStream(Stream Stream)
        {
            throw new NotImplementedException();
        }

        public override PDFWindow ViewURL(string URL)
        {
            throw new NotImplementedException();
        }

        protected override PDFWindow ViewFileEngine(string Path)
        {
            throw new NotImplementedException();
        }
    }
}
