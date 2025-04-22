using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class PDFJS : PDFEngine
    {
        public PDFJS(string ModuleFolder) : base(ModuleFolder)
        {


        }

        /// <inheritdoc />
        public override UpdateResult Update()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override PDFWindow ViewFileEngine(string Path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override PDFWindow ViewStream(Stream Stream)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override PDFWindow ViewURL(string URL)
        {
            throw new NotImplementedException();
        }
    }

}
