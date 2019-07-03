using System;
using System.Net.Http;
using System.Threading.Tasks;
using broadcast.Concurrency;

namespace broadcast.Http
{
    public class HttpPipeline: Actor<HttpRequest, HttpResponse>, IStream<HttpRequest, HttpResponse>
    {
        private HttpClient _httpClient;
        
        public HttpPipeline(HttpClient httpClient) : base()
        {
            _httpClient = httpClient;
        }

        protected override async Task<HttpResponse> DoSomethingToItem(HttpRequest request)
        {
            // here we convert an HttpRequest into a HttpResponse or return an error
            Console.WriteLine(request);
            var response = await request.execute(_httpClient);
            Console.WriteLine("Response received");
            Console.WriteLine(response);
            return response;
        }
        
    }
}