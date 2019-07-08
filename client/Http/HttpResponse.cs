using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace broadcast.Http
{
    public class HttpResponse
    {
        public HttpRequest request { get; private set; }
        
        public HttpResponseMessage response { get; private set; }
        public string body { get; private set; }
        public HttpStatusCode code { get; private set; }
        
        public Dictionary<string, string> headers { get; private set; }
        

        public bool ok => code.Equals(HttpStatusCode.OK);

        public HttpResponse(HttpRequest request, HttpResponseMessage response, string body, HttpStatusCode code)
        {
            this.request = request;
            this.response = response;
            this.body = body;
            this.code = code;
            if (this.response != null)
            {
                headers = this.response.Headers
                    .ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, string>(
                        (pair => pair.Key), (pair => string.Join("", pair.Value)));
            }
            
        }

        public override string ToString()
        {
            return "<" + this.request.type + " " + this.request.endpoint + ": " + this.code.ToString() + " " + this.body + ">";
        }
        
        public static HttpResponse ErrorToResponse(HttpRequest req, HttpRequestException e)
        {
            if (e.Message.Contains("500"))
            {
                return new HttpResponse(req, null, "Internal Server Error", code: HttpStatusCode.InternalServerError);
            }
            if (e.Message.Contains("501"))
            {
                return new HttpResponse(req, null, "Not Implemented", code: HttpStatusCode.NotImplemented);
            }
            if (e.Message.Contains("502"))
            {
                return new HttpResponse(req, null, "Gateway Error", code: HttpStatusCode.BadGateway);
            }
            if (e.Message.Contains("503"))
            {
                return new HttpResponse(req, null, "Service Unavailable", code: HttpStatusCode.ServiceUnavailable);
            }
            if (e.Message.Contains("504"))
            {
                return new HttpResponse(req, null, "Gateway Timeout", code: HttpStatusCode.GatewayTimeout);
            }
            if (e.Message.Contains("505"))
            {
                return new HttpResponse(req, null, "HTTP Version Not Supported", code: HttpStatusCode.HttpVersionNotSupported);
            }
            if (e.Message.Contains("400"))
            {
                return new HttpResponse(req, null, "Client Error", code: HttpStatusCode.BadRequest);
            }
            if (e.Message.Contains("401"))
            {
                return new HttpResponse(req, null, "Unauthorized", code: HttpStatusCode.Unauthorized);
            }
            if (e.Message.Contains("402"))
            {
                return new HttpResponse(req, null, "Payment Required", code: HttpStatusCode.PaymentRequired);
            }
            if (e.Message.Contains("403"))
            {
                return new HttpResponse(req, null, "Forbidden", code: HttpStatusCode.Forbidden);
            }
            if (e.Message.Contains("404"))
            {
                return new HttpResponse(req, null, "Not Found", code: HttpStatusCode.NotFound);
            }
            if (e.Message.Contains("405"))
            {
                return new HttpResponse(req, null, "Method Not Allowed", code: HttpStatusCode.MethodNotAllowed);
            }
            if (e.Message.Contains("406"))
            {
                return new HttpResponse(req, null, "UNACCEPTABLE!!", code: HttpStatusCode.NotAcceptable);
            }
            if (e.Message.Contains("409"))
            {
                return new HttpResponse(req, null, "Conflict", code: HttpStatusCode.Conflict);
            }
            if (e.Message.Contains("410"))
            {
                return new HttpResponse(req, null, "Gone", code: HttpStatusCode.Gone);
            }
            if (e.Message.Contains("413"))
            {
                return new HttpResponse(req, null, "Request Entity Too Large", code: HttpStatusCode.RequestEntityTooLarge);
            }
            // for some reason, C# doesn't include status codes for too many requests OR 418 I'M A TEAPOT
            //     what gives, Microsoft? 
            if (e.Message.Contains("418"))
            {
                return new HttpResponse(req, null, "I'm a teapot", HttpStatusCode.BadRequest);
            }
            if (e.Message.Contains("429"))
            {
                return new HttpResponse(req, null, "Cool your jets; Too many requests.", code: HttpStatusCode.BadRequest);
            }

            throw e;
        }
    }
}