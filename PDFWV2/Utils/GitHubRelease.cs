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
        public dynamic? ReleaseInfo { get; set; }
    }

    internal static class GitHubAPI
    {
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
    }
}
