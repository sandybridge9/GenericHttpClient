using Shared.GenericHttpClient.Models;
using Shared.GenericHttpClient.Wrappers;

namespace Shared.GenericHttpClient.Clients
{
    public class GenericClient() : IGenericClient
    {
        HttpClientWrapper httpClientWrapper = new HttpClientWrapper();

        public async Task<T?> GetDataFromUrlAsync<T>(string url) where T : class
        {
            var httpResponse = await httpClientWrapper.GetAsync<T>(url);

            return httpResponse is { ResponseType: HttpResponseType.Success, Data: not null }
                ? httpResponse.Data
                : default;
        }
    }
}
