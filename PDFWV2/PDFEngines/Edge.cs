using System.IO;

namespace PDFWV2.PDFEngines
{
    internal class Edge : PDFEngine
    {
        public Edge(string ModuleFolder) : base(ModuleFolder)
        {

        }
        /// <inheritdoc />
        public override UpdateResult Update()
        {
            // Edge PDF engine should always up to date.
            return UpdateResult.NoUpdate;
        }

        /// <inheritdoc />
        public override PDFWindow ViewFile(string Path)
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
