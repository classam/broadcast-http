using System;

namespace broadcast.Http
{
    public class HttpResponse
    {
        public HttpRequest request { get; private set; }
        public string body { get; private set; }
        public int code { get; private set; }

        public bool ok => code == 200;

        public HttpResponse(HttpRequest request, string body, int code)
        {
            this.request = request;
            this.body = body;
            this.code = code;
        }
    }
}