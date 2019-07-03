using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace broadcast.Socket
{
    public class Socket
    {
        private Uri _endpoint;
        private CancellationTokenSource _tokenSource;
        private ClientWebSocket _webSocket;
        
        public Socket(Uri endpoint){
            _tokenSource = new CancellationTokenSource();
            _webSocket = new ClientWebSocket();
            _endpoint = endpoint;
        }

        public async Task Connect()
        {
            await _webSocket.ConnectAsync(_endpoint, _tokenSource.Token);
        }

        public async Task Send(string message)
        {
            var byteMessage = System.Text.Encoding.UTF8.GetBytes(message);
            var byteMessageSegment = new ArraySegment<byte>(byteMessage);
            
            await _webSocket.SendAsync(byteMessageSegment, WebSocketMessageType.Text, true, _tokenSource.Token);
            return;
        }
        
        public async Task<string> Receive()
        {
            byte[] buffer = new byte[65536];
            var segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
            await _webSocket.ReceiveAsync(segment, _tokenSource.Token);
            
            string result = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            return result;
        }
    }
}