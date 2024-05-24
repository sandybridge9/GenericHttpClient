using Shared.GenericHttpClient.Models;
using System.Text;
using System.Text.Json;

namespace Shared.GenericHttpClient.Wrappers
{
    public class HttpClientWrapper() : IHttpClientWrapper
    {
        HttpClient httpClient = new HttpClient();

        public async Task<HttpResponse<T>> GetAsync<T>(string url) where T : class
        {
            var response = await httpClient.GetAsync(url);

            return await HandleResponseAsync<T>(response);
        }

        public async Task<HttpResponse<T>> PostAsync<T>(string url, object payload) where T : class
        {
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            return await HandleResponseAsync<T>(response);
        }

        public async Task<HttpResponse<T>> PutAsync<T>(string url, object payload) where T : class
        {
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(url, content);

            return await HandleResponseAsync<T>(response);
        }

        public async Task<HttpResponse<bool>> DeleteAsync(string url)
        {
            var httpResponse = new HttpResponse<bool>();
            var response = await httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                httpResponse.ResponseType = HttpResponseType.Failure;
                httpResponse.Message = string.IsNullOrWhiteSpace(response.ReasonPhrase)
                    ? $"Status code: {response.StatusCode}. API Call failed."
                    : $"Status code: {response.StatusCode}. {response.ReasonPhrase}.";

                return httpResponse;
            }

            httpResponse.ResponseType = HttpResponseType.Success;
            httpResponse.Data = true;

            return httpResponse;
        }

        private async Task<HttpResponse<T>> HandleResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            var httpResponse = new HttpResponse<T>();

            if (!response.IsSuccessStatusCode)
            {
                httpResponse.ResponseType = HttpResponseType.Failure;
                httpResponse.Message = string.IsNullOrWhiteSpace(response.ReasonPhrase)
                    ? $"Status code: {response.StatusCode}. API Call failed."
                    : $"Status code: {response.StatusCode}. {response.ReasonPhrase}.";

                return httpResponse;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                httpResponse.ResponseType = HttpResponseType.Empty;
                httpResponse.Message = $"Status code: {response.StatusCode}. API Call returned an empty response.";

                return httpResponse;
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(responseContent);

                httpResponse.ResponseType = HttpResponseType.Success;
                httpResponse.Data = data;

                return httpResponse;
            }
            catch (JsonException jsonException)
            {
                httpResponse.ResponseType = HttpResponseType.Undeserializable;
                httpResponse.Message = $"Status code: {response.StatusCode}. Failed to deserialize the API response to type {typeof(T)}. {jsonException.Message}.";

                return httpResponse;
            }
        }
    }
}
