using System.Net.Http;

namespace broadcast.Http
{
    public class HttpClient:System.Net.Http.HttpClient
    {
        public HttpClient()
        {
        }

        public HttpClient(HttpMessageHandler handler) : base(handler)
        {
        }

        public HttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
        }
    }
}