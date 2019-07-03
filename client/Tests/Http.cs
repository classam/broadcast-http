using System;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using broadcast.Http;
using NUnit.Framework;

namespace broadcast.Tests
{
    [TestFixture]
    public class HttpTest
    {
        public static string Endpoint = "http://localhost:40000";
        
        [Test]
        public async Task HelloWorld()
        {
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/hello"), null);
            var response = await request.execute(new HttpClient());
            Assert.IsNotEmpty(response.body);
            Assert.AreEqual(response.body, "Hello World!");
            return;
        }
         
        [Test]
        public async Task HeadersRequired()
        {
            var client = new HttpClient();
            // note that this change is permanently applied to the client
            // (so once we've applied cookies, they STAY applied)
            client.DefaultRequestHeaders.Add("X-BaconPancakes","BaconPancakes");
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/headers"), null);
            var response = await request.execute(client);
            Assert.IsNotEmpty(response.body);
            Assert.AreEqual(response.body, "Makin' Bacon Pancakes");
            return;
        }
        
        [Test]
        public async Task ServerError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/serverError"), null);
            var response = await request.execute(client);
            Assert.AreEqual(response.code, 500);
            return;
        }
        
        [Test]
        public async Task ClientError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/clientError"), null);
            var response = await request.execute(client);
            Assert.AreEqual(response.code, 400);
            return;
        }
        
        [Test]
        public async Task ForbiddenError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/forbidden"), null);
            var response = await request.execute(client);
            Assert.AreEqual(response.code, 403);
            return;
        }
        
        [Test]
        public async Task NotFoundError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/notFound"), null);
            var response = await request.execute(client);
            Assert.AreEqual(response.code, 404);
            return;
        }
        
        [Test]
        public async Task TimeoutError()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/timeout"), null);
            var response = await request.execute(client);
            Assert.AreEqual(response.code, 504);
            return;
        }
        
        [Test]
        public async Task ServerMissingError()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            
            var request = new HttpRequest(HttpRequest.GET, new Uri("https://sassages.missing/where"), null);
            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                var response = await request.execute(client);
            });
        }
        
        [Test]
        public async Task Post()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.POST, new Uri(Endpoint + "/test/echo"), "{\"hello\":\"there\"}");
            var response = await request.execute(client);
            
            Assert.AreEqual(200, response.code);
            Assert.AreEqual("{\"hello\":\"there\"}", response.body);
        }
        
        [Test]
        public async Task Put()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.PUT, new Uri(Endpoint + "/test/echo"), "{\"hello\":\"there\"}");
            var response = await request.execute(client);
            
            Assert.AreEqual(200, response.code);
            Assert.AreEqual("{\"hello\":\"there\"}", response.body);
        }
        
        [Test] 
        public async Task Delete()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.DELETE, new Uri(Endpoint + "/test/delete"), "");
            var response = await request.execute(client);
            
            Assert.AreEqual(200, response.code);
            Assert.AreEqual("OK", response.body);
        }
        
        [Test] 
        public async Task GetThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Get("test/hello", (response) =>
            {
                Assert.AreEqual(200, response.code);
                Assert.AreEqual("Hello World!", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.AreEqual(true, thisHappened);
        }
        
        [Test] 
        public async Task PutThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Put("test/echo", "{\"hello\": \"world\"}", (response) =>
            {
                Assert.AreEqual(200, response.code);
                Assert.AreEqual("{\"hello\":\"world\"}", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.AreEqual(true, thisHappened);
        }
        
        [Test] 
        public async Task PostThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Post("test/echo", "{\"hello\": \"world\"}", (response) =>
            {
                Assert.AreEqual(200, response.code);
                Assert.AreEqual("{\"hello\":\"world\"}", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.AreEqual(true, thisHappened);
        }
        
        [Test] 
        public async Task DeleteThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Delete("test/delete", (response) =>
            {
                Assert.AreEqual(200, response.code);
                Assert.AreEqual("OK", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.AreEqual(true, thisHappened);
        }
    }
}