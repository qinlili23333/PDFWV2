using System.IO;

namespace PDFWV2
{
    internal class PDFEngine
    {
        /// <summary>
        /// Initialize a PDF Engine.
        /// </summary>
        /// <param name="ModuleFolder">Folder to store module files</param>
        public PDFEngine(string ModuleFolder)
        {
            throw new NotImplementedException();
        }

        public enum UpdateResult
        {
            Done, //No running viewer, directly update
            PendingRestart, //Has running viewer, update at next launch
            NoUpdate, //Checked, but no update found
            FileError, //Local file in use or disk system errors
            NetError //Fail on network requests
        }

        /// <summary>
        /// Trigger update of the engine, return in UpdateResult.
        /// If EnableUpdate is not Never, then this method will be called automatically.
        /// </summary>
        public UpdateResult Update()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Open local PDF file with specific path.
        /// </summary>
        /// <param name="Path">Path to PDF file</param>
        public PDFWindow ViewFile(string Path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Open PDF stream.
        /// </summary>
        /// <param name="Stream">Stream object</param>
        public PDFWindow ViewStream(Stream Stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Open PDF from any URL.
        /// </summary>
        /// <param name="URL">URL string</param>
        public PDFWindow ViewURL(string URL)
        {
            throw new NotImplementedException();
        }
    }
}
