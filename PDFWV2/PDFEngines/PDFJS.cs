using System.IO;
using System.Text.Json;

namespace PDFWV2.PDFEngines
{
    internal class PDFJS : PDFEngine
    {
        private string FolderPath = string.Empty;

        private EngineVersion Version;

        public PDFJS(string ModuleFolder) : base(ModuleFolder)
        {
            FolderPath = ModuleFolder + "\\PDFJS";
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
                using FileStream createStream = File.Create(FolderPath + "\\version.json");
                {
                    JsonSerializer.Serialize(createStream, new EngineVersion() { Name = "pdfjs" });
                    Version = new EngineVersion();
                }
            }
            else
            {
                Version = JsonSerializer.Deserialize<EngineVersion>(FolderPath + "\\version.json") ?? new EngineVersion();
            }
            Update();
        }

        /// <inheritdoc />
        public override async Task<UpdateResult> Update()
        {
            // Check and download latest version
            return UpdateResult.Done;
        }

        /// <inheritdoc />
        protected override PDFWindow ViewFileEngine(string Path)
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

        /// <inheritdoc />
        public override bool IsReady()
        {
            if (Version.Version == 0)
            {
                return false;
            }
            else
            {
                if (PDFWV2InstanceManager.Options.EnableUpdate == UpdateMode.Foreground && Version.UpdateTime != DateTime.Now.Date.ToLongDateString())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

}
