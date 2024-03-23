using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.P2PNetwork
{
    public class Server : IDisposable
    {
        private TcpListener? _tcplistener;
        private TcpClient? _tcpclient;
        private Protocol? _protocol;
        private string? _name;
        private int _port;

        public Server(string name, int port)
        { 
            _name = name;
            _port = port;
        }

        public void Dispose()
        {
            _protocol.Dispose();
            Console.WriteLine($"Stopping TCP listener..");
            _tcpclient.Close();
            _tcplistener.Stop();
        }

        public async Task Listen()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
            _tcplistener = new TcpListener(ipEndPoint);

            Console.WriteLine($"Started TCP listener..");
            _tcplistener.Start();

            

            _tcpclient = await _tcplistener.AcceptTcpClientAsync();
            _protocol = new Protocol(_tcpclient.GetStream());
        }

        public async Task StartReading()
        {
            await Task.Run(async () =>
            {
                List<string> messages = null;
                do
                {
                    messages = await _protocol.ReadMessages();
                    foreach (var message in messages)
                    {
                        Console.WriteLine($"Server [{_name}] message received: {message}");
                    }
                }
                while (messages.Count > 0 && !messages.Contains("close connection"));
            });
        }

        public async Task StartSending()
        {
            await Task.Run(async () =>
            {
                await SendMessage($"📅 This is current time now: {DateTime.Now}");

                await SendMessage("Lets talk");

                await SendMessage("Do you understand me?");

                await SendMessage("What!?");

                await SendMessage("Why do you?");

                await SendMessage("close connection");
            });
        }

        private async Task SendMessage(string message)
        {
            await _protocol.SendMessage(message);

            Console.WriteLine($"Server [{_name}] message sent: {message}");
        }
    }
}
