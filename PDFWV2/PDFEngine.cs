using System.IO;

namespace PDFWV2
{
    /// <summary>
    /// Structure for engine version JSON file.
    /// Not all engines will use this.
    /// </summary>
    public class EngineVersion
    {
        /// <summary>
        /// Name of the engine.
        /// </summary>
        public string Name { get; set; } = "Default";
        /// <summary>
        /// Version of the engine in long.
        /// </summary>
        public long Version { get; set; } = 0;
        /// <summary>
        /// Last time update successfully performed.
        /// </summary>
        public string UpdateTime { get; set; } = DateTime.Now.Date.ToLongDateString();
        /// <summary>
        /// If there is an update package pending extraction, record version here.
        /// </summary>
        public long PkgVersion { get; set; } = 0;
    }

    public abstract class PDFEngine
    {
        /// <summary>
        /// Initialize a PDF Engine.
        /// </summary>
        /// <param name="ModuleFolder">Folder to store module files</param>
        public PDFEngine(string ModuleFolder)
        {
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
        public abstract Task<UpdateResult> Update();

        /// <summary>
        /// Whether this engine is ready to use.
        /// Ready means the engine is installed, and finished update if foreground update check is enabled.
        /// </summary>
        /// <returns>Whether is ready in bool</returns>
        public abstract bool IsReady();

        /// <summary>
        /// Open local PDF file with specific path with current engine.
        /// </summary>
        /// <param name="Path">Path to PDF file</param>
        protected abstract PDFWindow ViewFileEngine(string Path);

        /// <summary>
        /// Open local PDF file with specific path.
        /// Throws exception if it's not a PDF file.
        /// </summary>
        /// <param name="Path">Path to PDF file</param>
        public virtual PDFWindow ViewFile(string Path)
        {
            // Check whether it's PDF file first to avoid suspecious attacks
            if (PDFWV2InstanceManager.Options.SecurityHardenLevel == SecurityLevel.None || Utils.PDFHelper.IsPdf(Path))
            {
                return ViewFileEngine(Path);
            }
            else
            {
                throw new Exception("Not a PDF file.");
            }
        }

        /// <summary>
        /// Open PDF stream.
        /// </summary>
        /// <param name="Stream">Stream object</param>
        public abstract PDFWindow ViewStream(Stream Stream);

        /// <summary>
        /// Open PDF from any URL.
        /// </summary>
        /// <param name="URL">URL string</param>
        public abstract PDFWindow ViewURL(string URL);
    }
}
