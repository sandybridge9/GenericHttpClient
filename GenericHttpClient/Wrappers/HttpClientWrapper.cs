using Shared.GenericHttpClient.Models;
using System.Text.Json;

namespace Shared.GenericHttpClient.Wrappers
{
    public class HttpClientWrapper() : IHttpClientWrapper
    {
        HttpClient httpClient = new HttpClient();

        public async Task<HttpResponse<T>> PerformApiCallAsync<T>(string url) where T : class
        {
            var httpResponse = new HttpResponse<T>();

            var response = await httpClient.GetAsync(url);

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
                httpResponse.Message = string.IsNullOrWhiteSpace(response.ReasonPhrase)
                    ? $"Failed to deserialize the API response to type {typeof(T)}. {response.StatusCode}"
                    : response.ReasonPhrase;

                return httpResponse;
            }
        }
    }
}
