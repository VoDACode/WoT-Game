using System.Text;

namespace WoTConsole.Services
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
        public async static Task<HttpResponseMessage> Post(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Accept", "application/json");
            return await client.SendAsync(request);
        }
        public async static Task<string> Get(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            return await client.Send(request).Content.ReadAsStringAsync();
        }
        public async static Task<string> GetString(this HttpResponseMessage response)
        {
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
