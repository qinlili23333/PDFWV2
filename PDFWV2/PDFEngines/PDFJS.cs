using PDFWV2.Utils;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;

namespace PDFWV2.PDFEngines
{
    internal class PDFJS : PDFEngine
    {
        private string FolderPath = string.Empty;

        private EngineVersion Version;

        internal TaskCompletionSource<bool> ReadyTCS = new();

        private bool FallbackMode = false;

        private Edge FallbackEngine;

        public PDFJS(string ModuleFolder) : base(ModuleFolder)
        {
            FolderPath = ModuleFolder + "\\PDFJS";
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            if (!File.Exists(FolderPath + "\\version.json"))
            {
                using FileStream createStream = File.Create(FolderPath + "\\version.json");
                {
                    JsonSerializer.Serialize(createStream, new EngineVersion() { Name = "pdfjs" });
                    Version = new EngineVersion();
                }
            }
            else
            {
                Version = JsonSerializer.Deserialize<EngineVersion>(File.ReadAllText(FolderPath + "\\version.json")) ?? new EngineVersion();
            }
            InitAsync();
        }


        private async Task InitAsync()
        {
            if (File.Exists(FolderPath + "\\update.zip"))
            {
                await InstallPkg();
            }
            else if (Version.Version == 0 || (PDFWV2InstanceManager.Options.EnableUpdate != UpdateMode.Never && Version.UpdateTime != DateTime.Now.Date.ToLongDateString()))
            {
                UpdateResult Result = await Update();
                if (Version.Version == 0 && (Result == UpdateResult.FileError || Result == UpdateResult.NetError))
                {
                    if (PDFWV2InstanceManager.Options.FallbackToEdge)
                    {
                        FallbackMode = true;
                        FallbackEngine = new Edge();
                        PDFWV2InstanceManager.ActiveEngines[Engines.EDGE]= FallbackEngine;
                    }
                    else
                    {
                        throw new Exception("Fail to load PDF.JS dist.");
                    }
                    ReadyTCS.TrySetResult(false);
                }
                else
                {
                    ReadyTCS.TrySetResult(true);
                }
            }
        }

        /// <summary>
        /// Save current version information to JSON file
        /// </summary>
        /// <returns></returns>
        private async Task SaveVersion()
        {
            using FileStream createStream = File.Create(FolderPath + "\\version.json");
            {
                await JsonSerializer.SerializeAsync(createStream, Version);
            }
        }

        /// <summary>
        /// Install downloaded update package
        /// </summary>
        /// <returns></returns>
        private async Task InstallPkg()
        {
            try
            {
                ZipFile.ExtractToDirectory(FolderPath + "\\update.zip", FolderPath, true);
            }
            catch (Exception)
            {
                // Just remove failed file and redownload next time
                File.Delete(FolderPath + "\\update.zip");
            }
            Version.Version = Version.PkgVersion;
            Version.UpdateTime = DateTime.Now.Date.ToLongDateString();
            Version.PkgVersion = 0;
            File.Delete(FolderPath + "\\update.zip");
            await SaveVersion();
        }

        /// <inheritdoc />
        public override async Task<UpdateResult> Update()
        {
            // Check and download latest version
            GitHubRelease Latest = await GitHubAPI.GetLatestRelease(new GitHubRepo() { Owner = "Mozilla", Repo = "pdf.js" });
            if (Latest.Success)
            {
                long LatestVer = GitHubAPI.GetIdFromRelease(Latest);
                if (LatestVer > Version.Version)
                {
                    // Found new version
                    JsonElement? Dist = GitHubAPI.GetFileFromRelease(Latest, "pdfjs-(\\d+\\.)+\\d+-dist\\.zip");
                    if (Dist != null)
                    {
                        string DownloadURL = GitHubAPI.GetLinkFromFile((JsonElement)Dist);
                        HttpClient httpClient = new();
                        try
                        {
                            Stream fileStream = await httpClient.GetStreamAsync(DownloadURL);
                            // If no live instance or no installed version, directly install
                            if (Version.Version == 0 || PDFWV2InstanceManager.ActiveDocuments.Count == 0)
                            {
                                ZipFile.ExtractToDirectory(fileStream, FolderPath, true);
                                Version.Version = LatestVer;
                                Version.UpdateTime = DateTime.Now.Date.ToLongDateString();
                                await SaveVersion();
                                return UpdateResult.Done;
                            }
                            else
                            {
                                // Avoid race condition, save package and perform update next time launch
                                if (File.Exists(FolderPath + "\\update.zip"))
                                {
                                    File.Delete(FolderPath + "\\update.zip");
                                }
                                FileStream UpdatePkg = File.OpenWrite(FolderPath + "\\update.zip");
                                await fileStream.CopyToAsync(UpdatePkg);
                                UpdatePkg.Close();
                                fileStream.Close();
                                Version.PkgVersion = LatestVer;
                                Version.UpdateTime = DateTime.Now.Date.ToLongDateString();
                                await SaveVersion();
                                return UpdateResult.PendingRestart;
                            }
                        }
                        catch (Exception e) when (e is HttpRequestException || e is InvalidDataException)
                        {
                            return UpdateResult.NetError;
                        }
                        catch (Exception e) when (e is IOException || e is PathTooLongException || e is DirectoryNotFoundException || e is UnauthorizedAccessException)
                        {
                            return UpdateResult.FileError;
                        }
                    }
                    else
                    {
                        return UpdateResult.NetError;
                    }
                }
                else
                {
                    Version.UpdateTime = DateTime.Now.Date.ToLongDateString();
                    await SaveVersion();
                    return UpdateResult.NoUpdate;
                }
            }
            else if (Latest.ErrorMsg.Contains("limit"))
            {
                // Exceed GitHub 60 request per hour limit
                // I don't want to use my GitHub Token here
                // TODO: use hidden webview2 open release page and do manual parse
                return UpdateResult.NetError;
            }
            else
            {
                return UpdateResult.NetError;
            }
        }


        public override PDFWindow ViewFile(string Path)
        {
            if (FallbackMode)
            {
                FallbackEngine.ViewFile(Path);
            }
            return base.ViewFile(Path);
        }
        /// <inheritdoc />
        protected override PDFWindow ViewFileEngine(string Path)
        {
            if (FallbackMode)
            {
                return new PDFWindow(new EdgeController(new System.Uri(Path).AbsoluteUri));
            }
            return new PDFWindow(new PDFJSController(FolderPath, Path));
        }

        /// <inheritdoc />
        public override PDFWindow ViewStream(Stream Stream)
        {
            if (FallbackMode)
            {
                FallbackEngine.ViewStream(Stream);
            }
            return new PDFWindow(new PDFJSController(FolderPath));
        }

        /// <inheritdoc />
        public override PDFWindow ViewURL(string URL)
        {
            if (FallbackMode)
            {
                FallbackEngine.ViewURL(URL);
            }
            return new PDFWindow(new PDFJSController(FolderPath, URL));
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
