using System;
using System.Net;
using System.Net.WebSockets;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace broadcast.Tests
{
    [TestFixture]
    public class SocketTest
    {
        public static string Endpoint = "ws://localhost:40000";
        
        [Test]
        public async Task Connect()
        {
            var sock = new Socket.Socket(new Uri(Endpoint));
            await sock.Connect();
            
            Thread.Sleep(1000);
            
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");
            await sock.Send("hello world");

            Console.WriteLine(await sock.Receive());

            Thread.Sleep(1000);
            
            return;
        }
        
    }
}