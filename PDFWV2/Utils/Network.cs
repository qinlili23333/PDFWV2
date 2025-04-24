using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PDFWV2.Utils
{
    /// <summary>
    /// Network utils
    /// </summary>
    internal static class Network
    {
        /// <summary>
        /// Set http client default header.
        /// </summary>
        /// <param name="client">HttpClient object</param>
        /// <param name="mimetype">Expected mimetype</param>
        internal static void SetHttpHeader(HttpClient client, string mimetype)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(mimetype));
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("qinlili23333-PDFWV2/" + typeof(PDFWV2Instance).Assembly.GetName().Version?.ToString());
        }

        /// <summary>
        /// Get HTTP content and turn into a memory stream.
        /// </summary>
        /// <param name="URL">URL string</param>
        /// <returns>MemoryStream</returns>
        internal static async Task<MemoryStream> GetHttpStream(string URL)
        {
            using HttpClient client = new();
            {
                SetHttpHeader(client, "application/pdf");
                var fileStream = await client.GetStreamAsync(URL);
                MemoryStream inMemoryCopy = new();
                fileStream.CopyTo(inMemoryCopy);
                fileStream.Close();
                return inMemoryCopy;
            }
        }
    }
}
