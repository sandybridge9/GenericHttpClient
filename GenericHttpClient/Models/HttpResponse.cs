namespace GenericHttpClient.Models
{
    public class HttpResponse<T>
    {
        public HttpResponseType ResponseType { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
    }
}
