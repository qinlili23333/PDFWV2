using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

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
        public JsonElement? ReleaseInfo { get; set; }
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
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                    client.DefaultRequestHeaders.UserAgent.Clear();
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("qinlili23333:PDFWV2"));
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
                long? id = Release.ReleaseInfo?.GetProperty("id").GetInt64();
                return id ?? 0;
            }
            return 0;
        }
    }
}
