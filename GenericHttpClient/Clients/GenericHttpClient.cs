using Shared.GenericHttpClient.Models;
using System.Text;
using System.Text.Json;

namespace Shared.GenericHttpClient.Wrappers
{
    public class GenericHttpClient()
        : IGenericHttpClient
    {
        HttpClient httpClient = new HttpClient();

        public async Task<HttpResponse<T>> SendRequestAsync<T>(
            string url,
            HttpRequestType httpRequestType,
            T? payload = default)
        {
            ValidateRequest(url, httpRequestType, payload);

            HttpResponseMessage httpResponseMessage;

            switch (httpRequestType)
            {
                case HttpRequestType.GET:
                    httpResponseMessage = await GetAsync(url);

                    break;
                case HttpRequestType.POST:
                    httpResponseMessage = await PostAsync(url, payload);

                    break;
                case HttpRequestType.PUT:
                    httpResponseMessage = await PutAsync(url, payload);

                    break;
                case HttpRequestType.DELETE:
                    httpResponseMessage = await DeleteAsync(url);

                    break;
                default:
                    throw new ArgumentException("Unsupported HTTP request type. ");
            }

            var httpResponse = await HandleHttpResponseMessageAsync<T>(httpResponseMessage, httpRequestType, url);

            return httpResponse;
        }

        private static void ValidateRequest<T>(
            string url,
            HttpRequestType httpRequestType,
            T? payload)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("The provided url is null, empty or whitespace. ");
            }

            if ((httpRequestType == HttpRequestType.POST
                || httpRequestType == HttpRequestType.PUT)
                && payload == null)
            {
                throw new ArgumentException("Payload cannot be null for HTTP requests of type POST and PUT. ");
            }

            if((httpRequestType == HttpRequestType.DELETE
                || httpRequestType == HttpRequestType.GET)
                && typeof(bool) != typeof(T))
            {
                throw new ArgumentException($"Expected provided generic type for {httpRequestType} to be bool, but found {typeof(T)} ");
            }
        }

        private async Task<HttpResponseMessage> GetAsync(string url)
        {
            var httpResponseMessage = await httpClient.GetAsync(url);

            return httpResponseMessage;
        }

        private async Task<HttpResponseMessage> PostAsync<T>(string url, T? payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);

            var stringContent = new StringContent(
                jsonPayload,
                Encoding.UTF8,
                "application/json");

            var httpResponseMessage = await httpClient.PostAsync(url, stringContent);

            return httpResponseMessage;
        }

        private async Task<HttpResponseMessage> PutAsync<T>(string url, T? payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                jsonPayload,
                Encoding.UTF8,
                "application/json");

            var httpResponseMessage = await httpClient.PutAsync(url, content);

            return httpResponseMessage;
        }

        private async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var httpResponseMessage = await httpClient.DeleteAsync(url);

            return httpResponseMessage;
        }

        private async Task<HttpResponse<T>> HandleHttpResponseMessageAsync<T>(
            HttpResponseMessage httpResponseMessage,
            HttpRequestType httpRequestType,
            string url)
        {
            var httpResponse = new HttpResponse<T>();

            if(!httpResponseMessage.IsSuccessStatusCode)
            {
                httpResponse.ResponseType = HttpResponseType.Failure;
                httpResponse.Message = string.IsNullOrWhiteSpace(httpResponseMessage.ReasonPhrase)
                    ? @$"Status code: {httpResponseMessage.StatusCode}.
                        Request of type {httpRequestType} failed for url: {url}."
                    : @$"Status code: {httpResponseMessage.StatusCode}.
                        Request of type {httpRequestType} failed for url: {url}.
                        {httpResponseMessage.ReasonPhrase}.";

                return httpResponse;
            }

            if(httpRequestType == HttpRequestType.GET)
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    httpResponse.ResponseType = HttpResponseType.Empty;
                    httpResponse.Message = $"Status code: {httpResponseMessage.StatusCode}. API Call returned an empty response for url: {url}.";

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
                    httpResponse.Message = @$"Status code: {httpResponseMessage.StatusCode}.
                        Failed to deserialize the API response to type {typeof(T)} for url: {url}.
                        {jsonException.Message}.";

                    return httpResponse;
                }
            }

            httpResponse.ResponseType = HttpResponseType.Success;

            return httpResponse;
        }
    }
}
