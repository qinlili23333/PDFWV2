using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("qinlili23333:PDFWV2"));
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
