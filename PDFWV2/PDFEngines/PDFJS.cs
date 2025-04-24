using System.IO;
using System.Text.Json;

namespace PDFWV2.PDFEngines
{
    internal class PDFJS : PDFEngine
    {
        private string FolderPath = string.Empty;

        public PDFJS(string ModuleFolder) : base(ModuleFolder)
        {
            FolderPath = ModuleFolder + "\\PDFJS";
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
                using FileStream createStream = File.Create(FolderPath + "\\version.json");
                {
                    JsonSerializer.Serialize(createStream, new EngineVersion() { Name = "pdfjs" });
                }
            }

        }

        /// <inheritdoc />
        public override UpdateResult Update()
        {
            throw new NotImplementedException();
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

        public override bool IsReady()
        {
            throw new NotImplementedException();
        }
    }

}
