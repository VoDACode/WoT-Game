using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WoTWpfClient.Services
{
    static class Http
    {
        private static HttpClient client
        {
            get
            {
                var h = new HttpClientHandler();
                h.ClientCertificateOptions = ClientCertificateOption.Manual;
                h.ServerCertificateCustomValidationCallback += (s, ce, ch, sslPE) => true;
                return new HttpClient(h);
            }
        }
        public static HttpResponseMessage Post(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            return client.SendAsync(request).Result;
        }
        public async static Task<string> Get(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            return await client.Send(request).Content.ReadAsStringAsync();
        }
        public async static Task<string> GetString(this HttpResponseMessage response)
        {
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
