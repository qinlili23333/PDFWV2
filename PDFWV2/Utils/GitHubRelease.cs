using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PDFWV2.Utils
{
    internal class GitHubRepo
    {
        public string Owner { get; set; } = string.Empty;
        public string Repo { get; set; } = string.Empty;
    }

    internal class GitHubRelease
    {
        public bool Success { get; set; } = false;
        public string ErrorMsg { get; set; } = string.Empty;
        public JsonElement ReleaseInfo { get; set; }
    }


    internal static class GitHubAPI
    {
        /// <summary>
        /// Get latest release of specific repository.
        /// Will return a GitHubRelease object with Success set to false if failed.
        /// </summary>
        /// <param name="Repo">A GitHubRepo object</param>
        /// <returns>A GitHubRelease object</returns>
        public async static Task<GitHubRelease> GetLatestRelease(GitHubRepo Repo)
        {
            try
            {
                using HttpClient client = new();
                {
                    Network.SetHttpHeader(client, "application/vnd.github+json");
                    var jsonStream = await client.GetStreamAsync($"https://api.github.com/repos/{Repo.Owner}/{Repo.Repo}/releases?per_page=1");
                    var jsonDoc = await JsonDocument.ParseAsync(jsonStream);
                    return new GitHubRelease
                    {
                        Success = true,
                        ReleaseInfo = jsonDoc.RootElement[0]
                    };
                }
            }
            catch (Exception ex)
            {
                return new GitHubRelease
                {
                    Success = false,
                    ErrorMsg = ex.Message
                };
            }
        }

        /// <summary>
        /// Extract long id from a release.
        /// Return 0 if the release is unsuccessful.
        /// </summary>
        /// <param name="Release">A GitHubRelease object get from GetLatestRelease</param>
        /// <returns>Long id</returns>
        public static long GetIdFromRelease(GitHubRelease Release)
        {
            if (Release.Success)
            {
                return Release.ReleaseInfo.GetProperty("id").GetInt64();
            }
            return 0;
        }

        /// <summary>
        /// Get a target file from release based on file name regex.
        /// </summary>
        /// <param name="Release">GitHubRelease object</param>
        /// <param name="RegEx">RegEx to filter file name, optional</param>
        /// <returns>A JsonElement of the target file, or null if no file found</returns>
        public static JsonElement? GetFileFromRelease(GitHubRelease Release, string RegEx = "(.*?)")
        {
            if (Release.Success)
            {
                var assets = Release.ReleaseInfo.GetProperty("assets").EnumerateArray();
                //GitHub returns empty assets array if no file, so should not need to catch KeyNotFoundException
                //So just detect length
                if (assets.Count<JsonElement>() == 0)
                {
                    return null;
                }
                foreach (JsonElement asset in assets)
                {
                    if (Regex.Match(asset.GetProperty("name").GetString() ?? "", RegEx).Success)
                    {
                        return asset;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get download link from file object
        /// </summary>
        /// <param name="File">JsonElement object from GetFileFromRelease</param>
        /// <returns>Link string</returns>
        public static string GetLinkFromFile(JsonElement File)
        {
            return File.GetProperty("browser_download_url").GetString() ?? string.Empty;
        }
    }
}
