using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskWpf.Properties;

namespace TaskWpf.Helpers
{
    static class WebApiHelper
    {
        private static string JwtToken = UserSettings.Default.JwtToken;

        public async static Task<HttpResponseMessage> SendRequestAsync
            (HttpMethod httpMethod, string url, string requestContent)
        {
            var requestRsult = new HttpResponseMessage();

            // Bypass the certificate
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, url);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtToken);
                requestMessage.Content = new StringContent(requestContent, Encoding.UTF8, "application/json");
                requestRsult = await client.SendAsync(requestMessage);
            }
            return requestRsult;
        }

        public async static Task<HttpResponseMessage> SendRequestAsync
            (HttpMethod httpMethod, string url)
        {
            return await SendRequestAsync(httpMethod, url, "");
        }

        public static void UpdateJwtToken(string newToken)
        {
            JwtToken = newToken;
            UserSettings.Default.JwtToken = newToken;
            UserSettings.Default.Save();
        }
    }
}
