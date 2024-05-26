namespace GenericHttpClient.Clients
{
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// Method wrapping System.Net.Http.HttpClient.GetAsync().
        /// </summary>
        /// <param name="url">URL to be called.</param>
        /// <returns>HttpResponseMessage.</returns>
        public Task<HttpResponseMessage> GetAsync(string url);

        /// <summary>
        /// Method wrapping System.Net.Http.HttpClient.PostAsync().
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="url">URL to be called.</param>
        /// <param name="payload">Payload to be passed.</param>
        /// <returns>HttpResponseMessage.</returns>
        public Task<HttpResponseMessage> PostAsync<T>(string url, T? payload);

        /// <summary>
        /// Method wrapping System.Net.Http.HttpClient.PutAsync().
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="url">URL to be called.</param>
        /// <param name="payload">Payload to be passed.</param>
        /// <returns>HttpResponseMessage.</returns>
        public Task<HttpResponseMessage> PutAsync<T>(string url, T? payload);

        /// <summary>
        /// Method wrapping System.Net.Http.HttpClient.DeleteAsync().
        /// </summary>
        /// <param name="url">URL to be called.</param>
        /// <returns>HttpResponseMessage.</returns>
        public Task<HttpResponseMessage> DeleteAsync(string url);
    }
}
