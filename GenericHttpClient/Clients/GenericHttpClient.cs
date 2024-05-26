using GenericHttpClient.Models;
using System.Text.Json;

namespace GenericHttpClient.Clients
{
    public class GenericHttpClient(IHttpClientWrapper httpClientWrapper)
        : IGenericHttpClient
    {
        public async Task<HttpResponse<T>> SendRequestAsync<T>(
            string url,
            HttpRequestType httpRequestType,
            T? payload = default)
        {
            ValidateRequest(url, httpRequestType, payload);

            HttpResponseMessage httpResponseMessage = httpRequestType switch
            {
                HttpRequestType.GET => await httpClientWrapper.GetAsync(url),
                HttpRequestType.POST => await httpClientWrapper.PostAsync(url, payload),
                HttpRequestType.PUT => await httpClientWrapper.PutAsync(url, payload),
                HttpRequestType.DELETE => await httpClientWrapper.DeleteAsync(url),
                _ => throw new NotImplementedException(),
            };

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

            if (httpRequestType == HttpRequestType.DELETE
                && typeof(bool) != typeof(T))
            {
                throw new ArgumentException($"Expected provided generic type for {httpRequestType} to be bool, but found {typeof(T)} ");
            }
        }

        private async Task<HttpResponse<T>> HandleHttpResponseMessageAsync<T>(
            HttpResponseMessage httpResponseMessage,
            HttpRequestType httpRequestType,
            string url)
        {
            var httpResponse = new HttpResponse<T>();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                httpResponse.ResponseType = HttpResponseType.Failure;
                httpResponse.Message = string.IsNullOrWhiteSpace(httpResponseMessage.ReasonPhrase)
                    ? @$"Status code: {httpResponseMessage.StatusCode}.
                        Request of type {httpRequestType} failed for url: {url}."
                    : @$"Status code: {httpResponseMessage.StatusCode}.
                        Request of type {httpRequestType} failed for url: {url}.
                        {httpResponseMessage.ReasonPhrase}.";

                if (typeof(T) == typeof(bool))
                {
                    httpResponse.Data = (T)(object)false;
                }

                return httpResponse;
            }

            if (httpRequestType == HttpRequestType.GET)
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

            if(typeof(T) == typeof(bool))
            {
                httpResponse.Data = (T)(object)true;
            }

            return httpResponse;
        }
    }
}
