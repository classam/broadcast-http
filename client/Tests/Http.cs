using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using broadcast.Http;
using NUnit.Framework;
using HttpClient = broadcast.Http.HttpClient;

namespace broadcast.Tests
{
    [TestFixture]
    public class HttpTest
    {
        public static string Endpoint = "http://localhost:40000";
        
        [Test]
        public async Task HelloWorld()
        {
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/hello"));
            var response = await request.execute(new HttpClient());
            Assert.IsNotEmpty(response.body);
            Assert.AreEqual("Hello World!", response.body);
            return;
        }
         
        [Test]
        public async Task HeadersRequired()
        {
            var client = new HttpClient();
            // note that this change is permanently applied to the client
            // (so once we've applied cookies, they STAY applied)
            client.DefaultRequestHeaders.Add("X-BaconPancakes","BaconPancakes");
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/headers"));
            var response = await request.execute(client);
            Assert.IsNotEmpty(response.body);
            Assert.AreEqual("Makin' Bacon Pancakes", response.body);
            return;
        }
        
        [Test]
        public async Task ServerError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/serverError"));
            var response = await request.execute(client);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.code);
            return;
        }
        
        [Test]
        public async Task ClientError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/clientError"));
            var response = await request.execute(client);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.code);
            return;
        }
        
        [Test]
        public async Task PostClientError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.POST, new Uri(Endpoint + "/test/clientError"), "{}");
            var response = await request.execute(client);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.code);
            return;
        }
        
        [Test]
        public async Task ForbiddenError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/forbidden"));
            var response = await request.execute(client);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.code);
            return;
        }
        
        [Test]
        public async Task NotFoundError()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/notFound"));
            var response = await request.execute(client);
            Assert.AreEqual(HttpStatusCode.NotFound, response.code);
            return;
        }
        
        [Test]
        public async Task TimeoutError()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/timeout"));
            var response = await request.execute(client);
            Assert.AreEqual(HttpStatusCode.RequestTimeout, response.code);
            return;
        }
        
        [Test]
        public async Task ServerMissingError()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            
            var request = new HttpRequest(HttpRequest.GET, new Uri("https://sassages.missing/where"));
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
            
            Assert.AreEqual(HttpStatusCode.OK, response.code);
            Assert.AreEqual("{\"hello\":\"there\"}", response.body);
        }
        
        [Test]
        public async Task Put()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.PUT, new Uri(Endpoint + "/test/echo"), "{\"hello\":\"there\"}");
            var response = await request.execute(client);
            
            Assert.AreEqual(HttpStatusCode.OK, response.code);
            Assert.AreEqual("{\"hello\":\"there\"}", response.body);
        }
        
        [Test] 
        public async Task Delete()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.DELETE, new Uri(Endpoint + "/test/delete"));
            var response = await request.execute(client);
            
            Assert.AreEqual(HttpStatusCode.OK, response.code);
            Assert.AreEqual("OK", response.body);
        }
        
        [Test] 
        public async Task Patch()
        {
            var client = new HttpClient();
            
            var request = new HttpRequest(HttpRequest.PATCH, new Uri(Endpoint + "/test/echo"), "{\"hello\":\"there\"}");
            var response = await request.execute(client);
            
            Assert.AreEqual(HttpStatusCode.OK, response.code);
            Assert.AreEqual("{\"hello\":\"there\"}", response.body);
        }
        
        [Test] 
        public async Task Head()
        {
            var client = new HttpClient();
            
            client.DefaultRequestHeaders.Add("X-BaconPancakes","BaconPancakes");
            
            var request = new HttpRequest(HttpRequest.HEAD, new Uri(Endpoint + "/test/headers"));
            
            var response = await request.execute(client);
            
            Assert.AreEqual(HttpStatusCode.OK, response.code);
            Assert.IsTrue(response.response.Headers.Contains("ETag"));
            Assert.IsTrue(response.response.Headers.Contains("X-Powered-By"));
            Assert.IsTrue(response.response.Headers.Contains("X-BaconPancakes"));
            Assert.AreEqual("Makin' Bacon Pancakes", response.headers["X-BaconPancakes"]);
        }
        
        [Test] 
        public async Task GetThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Get("test/hello", (response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.code);
                Assert.AreEqual("Hello World!", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.IsTrue(thisHappened);
        }
        
        [Test] 
        public async Task PutThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Put("test/echo", "{\"hello\": \"world\"}", (response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.code);
                Assert.AreEqual("{\"hello\":\"world\"}", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.IsTrue(thisHappened);
        }
        
        [Test] 
        public async Task PostThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Post("test/echo", "{\"hello\": \"world\"}", (response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.code);
                Assert.AreEqual("{\"hello\":\"world\"}", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.IsTrue(thisHappened);
        }
        
        [Test] 
        public async Task PatchThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Patch("test/echo", "{\"hello\": \"world\"}", (response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.code);
                Assert.AreEqual("{\"hello\":\"world\"}", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.IsTrue(thisHappened);
        }
        
        [Test] 
        public async Task DeleteThreaded()
        {
            var http = new Http.Http(new Uri(Endpoint));

            var thisHappened = false;

            http.Delete("test/delete", (response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.code);
                Assert.AreEqual("OK", response.body);
                thisHappened = true;
            });

            http.TestWait(5000);
            Assert.IsTrue(thisHappened);
        }
        
        [Test] 
        public async Task FailBasicAuth()
        {
            var client = new HttpClient();
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/auth/basic"));
            var response = await request.execute(new HttpClient());
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.code);
        }
        
        [Test] 
        public async Task PassBasicAuth()
        {
            var client = new HttpClient();
            var username = "admin";
            var password = "hunter2";
            
            Http.Http.AddBasicAuth(client, username, password);
            var request = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + "/test/auth/basic"));
            var response = await request.execute(client);
            Assert.IsNotEmpty(response.body);
            Assert.AreEqual(HttpStatusCode.OK, response.code);
            Assert.AreEqual("Hello World!", response.body);
        }
        
        [Test] 
        public async Task TokenAuth()
        {
            // TODO
        }
        
        [Test] 
        public async Task FollowRedirects()
        {
            // TODO
        }
        
        [Test] 
        public async Task DontFollowRedirects()
        {
            // TODO
        }
        
        [Test] 
        public async Task ReceiveGzipped()
        {
            // TODO
        }
        
        [Test] 
        public async Task SendGzipped()
        {
            // TODO
        }
    }
}