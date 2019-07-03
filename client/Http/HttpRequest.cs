using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using broadcast.Errors;
using NUnit.Framework.Constraints;

namespace broadcast.Http
{
    public class HttpRequest
    {
        public static string GET = "GET";
        public static string POST = "POST";
        public static string PUT = "PUT";
        public static string DELETE = "DELETE";
        
        public Guid id { get; private set; }

        public string type { get; private set; }
        public Uri endpoint { get; private set; }
        public string data { get; private set; }
        
        public HttpRequest(string type, Uri endpoint, string data)
        {
            this.id = Guid.NewGuid();
            this.type = type;
            this.endpoint = endpoint;
            this.data = data;
        }

        public HttpResponse ErrorToResponse(HttpRequestException e)
        {
            if (e.Message.Contains("500"))
            {
                return new HttpResponse(this, "Internal Server Error", code: 500);
            }
            if (e.Message.Contains("501"))
            {
                return new HttpResponse(this, "Not Implemented", code: 501);
            }
            if (e.Message.Contains("502"))
            {
                return new HttpResponse(this, "Gateway Error", code: 502);
            }
            if (e.Message.Contains("503"))
            {
                return new HttpResponse(this, "Service Unavailable", code: 503);
            }
            if (e.Message.Contains("504"))
            {
                return new HttpResponse(this, "Gateway Timeout", code: 504);
            }
            if (e.Message.Contains("505"))
            {
                return new HttpResponse(this, "HTTP Version Not Supported", code: 505);
            }
            if (e.Message.Contains("400"))
            {
                return new HttpResponse(this, "Client Error", code: 400);
            }
            if (e.Message.Contains("401"))
            {
                return new HttpResponse(this, "Unauthorized", code: 401);
            }
            if (e.Message.Contains("402"))
            {
                return new HttpResponse(this, "Payment Required", code: 402);
            }
            if (e.Message.Contains("403"))
            {
                return new HttpResponse(this, "Forbidden", code: 403);
            }
            if (e.Message.Contains("404"))
            {
                return new HttpResponse(this, "Not Found", code: 404);
            }
            if (e.Message.Contains("405"))
            {
                return new HttpResponse(this, "Method Not Allowed", code: 405);
            }
            if (e.Message.Contains("406"))
            {
                return new HttpResponse(this, "UNACCEPTABLE!!", code: 405);
            }
            if (e.Message.Contains("409"))
            {
                return new HttpResponse(this, "Conflict", code: 409);
            }
            if (e.Message.Contains("410"))
            {
                return new HttpResponse(this, "Gone", code: 410);
            }
            if (e.Message.Contains("413"))
            {
                return new HttpResponse(this, "The Booty is Fat; Request Entity Too Large", code: 413);
            }
            if (e.Message.Contains("418"))
            {
                return new HttpResponse(this, "I'm a Teapot", code: 418);
            }
            if (e.Message.Contains("420"))
            {
                return new HttpResponse(this, "Blaze it", code: 420);
            }
            if (e.Message.Contains("429"))
            {
                return new HttpResponse(this, "Cool your jets; Too many requests.", code: 429);
            }

            throw e;
        }

        public async Task<HttpResponse> execute(HttpClient client)
        {
            if (this.type == GET)
            {
                try
                {
                    string responseBody = await client.GetStringAsync(this.endpoint.ToString());
                    return new HttpResponse(this, responseBody, 200);
                }
                catch (HttpRequestException e)
                {
                    return ErrorToResponse(e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, "Response Timeout", 504);
                }
            }
            else if (this.type == POST)
            {
                try
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var responseBody = await client.PostAsync(this.endpoint.ToString(), content);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseString, 200);
                }
                catch (HttpRequestException e)
                {
                    return ErrorToResponse(e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, "Response Timeout", 504);
                }
            }
            else if (this.type == PUT)
            {
                try
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var responseBody = await client.PutAsync(this.endpoint.ToString(), content);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseString, 200);
                }
                catch (HttpRequestException e)
                {
                    return ErrorToResponse(e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, "Response Timeout", 504);
                }
            }
            else if (this.type == DELETE)
            {
                try
                {
                    var responseBody = await client.DeleteAsync(this.endpoint.ToString());
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseString, 200);
                }
                catch (HttpRequestException e)
                {
                    return ErrorToResponse(e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, "Response Timeout", 504);
                }
            }
            else
            {
                throw new BroadcastError(this.type + " is not GET, PUT, POST, or DELETE, which are the only HTTP verbs we actually handle");
            }
        }
        
    }
}