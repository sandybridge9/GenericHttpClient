using Shared.GenericHttpClient.Models;

namespace Shared.GenericHttpClient.Wrappers
{
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// Performs a call using HTTP client to the provided URL.
        /// </summary>
        /// <param name="urlTemplate">URL template to be used in an HTTP client call.</param>
        /// <returns>HttpResponse containing response type and data.</returns>
        Task<HttpResponse<T>> GetAsync<T>(string url) where T : class;

        Task<HttpResponse<T>> PostAsync<T>(string url, object payload) where T : class;

        Task<HttpResponse<T>> PutAsync<T>(string url, object payload) where T : class;

        Task<HttpResponse<bool>> DeleteAsync(string url);
    }
}
