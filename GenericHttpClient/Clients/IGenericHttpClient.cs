using Shared.GenericHttpClient.Models;

namespace Shared.GenericHttpClient.Wrappers
{
    public interface IGenericHttpClient
    {
        /// <summary>
        /// Sends a request of provided to type to the provided url with the provided payload (optional).
        /// </summary>
        /// <param name="url">URL that will receive the request.</param>
        /// <param name="httpRequestType">HTTP request type.</param>
        /// <param name="payload">PAyload to be send with this request.</param>
        /// <returns>HttpResponse containing request status, response type, data and message (in case of failure).</returns>
        public Task<HttpResponse<T>> SendRequestAsync<T>(
            string url,
            HttpRequestType httpRequestType,
            T? payload = default);
    }
}
