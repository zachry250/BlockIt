using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockIt.P2PNetwork
{
    public class Server : ITCPConnection
    {
        private readonly string? _name;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        public Server(string name)
        { 
            _name = name;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public async Task Listen(int port, Func<Connection, Task> newConnectionCreated)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            var tcplistener = new TcpListener(ipEndPoint);

            Console.WriteLine($"Started TCP listener..");
            tcplistener.Start();
            TcpClient tcpClient;

            try
            {
                while (true)
                {
                    tcpClient = await tcplistener.AcceptTcpClientAsync(_cancellationToken);
                    await AcceptConnection(tcpClient, newConnectionCreated);
                }
            }
            finally 
            { 
                tcplistener.Stop();
            }
        }

        public void Close()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task AcceptConnection(TcpClient tcpClient, Func<Connection, Task> newConnectionCreated)
        {
            await Task.Yield();
            try
            {
                using (tcpClient)
                using (var connection = new Connection($"{_name} connection", this, new Protocol(tcpClient.GetStream())))
                {
                    await newConnectionCreated(connection);
                    await connection.RegisterMessageReceiver();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
