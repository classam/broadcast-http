using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using broadcast.Concurrency;
using broadcast.Errors;
using broadcast.Events;

namespace broadcast.Http
{
    class Http
    {
        public Uri Endpoint { get; private set; }

        private readonly HttpClient _httpClient;
        private readonly HttpPipeline _httpPipeline;

        private readonly Dictionary<Guid, Response> _responseDictionary;

        public delegate void Response(HttpResponse resp);

        public event EventHandler<BroadcastEvent<Exception>> OnError;
        public event EventHandler<BroadcastEvent<string>> OnLog;
        
        public Http(Uri endpoint)
        {
            Endpoint = endpoint;

            _httpClient = new HttpClient();

            _httpPipeline = new HttpPipeline(_httpClient);
            _responseDictionary = new Dictionary<Guid, Response>();
        }

        public Http(Uri endpoint, Dictionary<string, string> cookies)
        {
            Endpoint = endpoint;

            _httpClient = new HttpClient();

            foreach (var key in cookies.Keys)
            {
                _httpClient.DefaultRequestHeaders.Add(key, cookies[key]);
            }

            _httpPipeline = new HttpPipeline(_httpClient);
            _responseDictionary = new Dictionary<Guid, Response>();
        }

        /*
         * Update, called from unity, pulls one HttpResult out of the output queue and
         * calls any event handlers we have for it.
         * It will also push a log item and an error item from the queue.
         */
        public void Update()
        {
            Console.WriteLine("PIPELINE IS EMPTY " + _httpPipeline.IsEmpty() );
            
            var response = _httpPipeline.Out();
            if (response != null)
            {
                Handle(response);
            }

            var log = _httpPipeline.LogOut();
            if (log != null)
            {
                Handle(log);
            }

            var error = _httpPipeline.ErrOut();
            if (error != null)
            {
                Handle(error);
            }
        }

        public void Get(string path, Response responseDelegate)
        {
            var get = new HttpRequest(HttpRequest.GET, new Uri(Endpoint + path), "");

            _responseDictionary.Add(get.id, responseDelegate);

            _httpPipeline.In(get);
        }

        public void Post(string path, string data, Response responseDelegate)
        {
            var post = new HttpRequest(HttpRequest.POST, new Uri(Endpoint + path), data);

            _responseDictionary.Add(post.id, responseDelegate);

            _httpPipeline.In(post);
        }

        public void Put(string path, string data, Response responseDelegate)
        {
            var put = new HttpRequest(HttpRequest.PUT, new Uri(Endpoint + path), data);

            _responseDictionary.Add(put.id, responseDelegate);

            _httpPipeline.In(put);
        }

        public void Delete(string path, Response responseDelegate)
        {
            var delete = new HttpRequest(HttpRequest.DELETE, new Uri(Endpoint + path), "");

            _responseDictionary.Add(delete.id, responseDelegate);

            _httpPipeline.In(delete);
        }
        
        public void Head(string path, Response responseDelegate)
        {
            var head = new HttpRequest(HttpRequest.HEAD, new Uri(Endpoint + path), "");
            
            _responseDictionary.Add(head.id, responseDelegate);
            
            _httpPipeline.In(head);
        }
        
        public void Patch(string path, string data, Response responseDelegate)
        {
            var patch = new HttpRequest(HttpRequest.PATCH, new Uri(Endpoint + path), data);
            
            _responseDictionary.Add(patch.id, responseDelegate);
            
            _httpPipeline.In(patch);
        }

        private void Handle(string log)
        {
            Console.WriteLine(log);
            var updateHandler = OnLog;
            if (updateHandler != null)
            {
                updateHandler(this, new BroadcastEvent<string>(log));
            }
        }

        private void Handle(Exception error)
        {
            Console.WriteLine(error);
            var updateHandler = OnError;
            if (updateHandler != null)
            {
                updateHandler(this, new BroadcastEvent<Exception>(error));
            }
        }

        private void Handle(HttpResponse response)
        {
            Console.WriteLine(response);
            if (_responseDictionary.ContainsKey(response.request.id))
            {
                var delegato = _responseDictionary[response.request.id];
                delegato(response);
                _responseDictionary.Remove(response.request.id);
            }
            else
            {
                Handle(new BroadcastError("No delegate for " + response));
            }
        }

        public void TestWait(int milliseconds)
        {
            int updateInterval = 50;

            int counter = 0;
            while (counter < milliseconds)
            {
                Thread.Sleep(updateInterval);
                Update();
                counter += updateInterval;
            }
        }

    }
}