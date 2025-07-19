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
                Version = JsonSerializer.Deserialize<EngineVersion>(File.ReadAllText(FolderPath + "\\version.json")) ?? new EngineVersion();
            }
            if (Version.Version == 0 || PDFWV2InstanceManager.Options.EnableUpdate != UpdateMode.Never)
            {
                Update();
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
                            // If no live instance, directly install
                            if (PDFWV2InstanceManager.ActiveDocuments.Count == 0)
                            {
                                ZipFile.ExtractToDirectory(fileStream, FolderPath);
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
            else
            {
                return UpdateResult.NetError;
            }
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
