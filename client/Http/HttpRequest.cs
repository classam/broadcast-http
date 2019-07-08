using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
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
        public static string PATCH = "PATCH";
        public static string HEAD = "HEAD";
        
        public Guid id { get; private set; }

        public string type { get; private set; }
        public Uri endpoint { get; private set; }
        public string data { get; private set; }

        private HttpRequestMessage rawMessage;
        
        public HttpRequest(string type, Uri endpoint)
        {
            this.id = Guid.NewGuid();
            this.type = type;
            this.endpoint = endpoint;
            this.data = "";
        }

        public HttpRequest(string type, Uri endpoint, string data)
        {
            this.id = Guid.NewGuid();
            this.type = type;
            this.endpoint = endpoint;
            this.data = data;
        }

        public HttpRequest(HttpRequestMessage rawMessage)
        {
            this.id = Guid.NewGuid();
            this.type = rawMessage.Method.ToString();
            this.endpoint = rawMessage.RequestUri;
            this.data = rawMessage.Content.ToString();
            this.rawMessage = rawMessage;
        }

        public async Task<HttpResponse> execute(HttpClient client)
        {
            if (this.rawMessage != null)
            {
                try{
                    var responseBody = await client.SendAsync(this.rawMessage);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else if (this.type == GET)
            {
                try
                {
                    var responseBody = await client.GetAsync(this.endpoint.ToString());
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else if (this.type == POST)
            {
                try
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var responseBody = await client.PostAsync(this.endpoint.ToString(), content);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else if (this.type == PUT)
            {
                try
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var responseBody = await client.PutAsync(this.endpoint.ToString(), content);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else if (this.type == DELETE)
            {
                try
                {
                    var responseBody = await client.DeleteAsync(this.endpoint.ToString());
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else if (this.type == HEAD)
            {
                try
                {
                    var request = new HttpRequestMessage(new HttpMethod("HEAD"), this.endpoint);
                    var responseBody = await client.SendAsync(request);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else if (this.type == PATCH)
            {
                try
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint);
                    request.Content = content;
                    var responseBody = await client.SendAsync(request);
                    var responseString = await responseBody.Content.ReadAsStringAsync();
                    return new HttpResponse(this, responseBody, responseString, responseBody.StatusCode);
                }
                catch (HttpRequestException e)
                {
                    return HttpResponse.ErrorToResponse(this, e);
                }
                catch (System.Threading.Tasks.TaskCanceledException e)
                {
                    return new HttpResponse(this, null, "Response Timeout", HttpStatusCode.RequestTimeout);
                }
            }
            else
            {
                throw new BroadcastError(this.type + " is not GET, PUT, POST, PATCH, HEAD, or DELETE, which are the only HTTP verbs we actually handle");
            }
        }
        
    }
}