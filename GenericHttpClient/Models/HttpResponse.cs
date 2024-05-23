﻿namespace Shared.GenericHttpClient.Models
{
    public class HttpResponse<T>
    {
        public HttpResponseType ResponseType { get; set; }

        public string Message { get; set; }

        public T? Data { get; set; }
    }
}
