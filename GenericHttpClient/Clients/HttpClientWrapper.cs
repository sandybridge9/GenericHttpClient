using System.Text.Json;
using System.Text;

namespace GenericHttpClient.Clients
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var httpResponseMessage = await httpClient.GetAsync(url);

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T? payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);

            var stringContent = new StringContent(
                jsonPayload,
                Encoding.UTF8,
                "application/json");

            var httpResponseMessage = await httpClient.PostAsync(url, stringContent);

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T? payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                jsonPayload,
                Encoding.UTF8,
                "application/json");

            var httpResponseMessage = await httpClient.PutAsync(url, content);

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var httpResponseMessage = await httpClient.DeleteAsync(url);

            return httpResponseMessage;
        }
    }
}
