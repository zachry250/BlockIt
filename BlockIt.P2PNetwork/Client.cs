using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace BlockIt.P2PNetwork
{
    public class Client : IDisposable
    {
        private TcpClient? _tcpClient;
        private Protocol? _protocol;
        private string? _name;
        private int _port;

        public Client(string name, int port)
        {
            _name = name;
            _port = port;
        }

        public void Dispose()
        {
            _protocol.Dispose();
            _tcpClient.Close();
        }

        public async Task Connect()
        {
            _tcpClient = new TcpClient();
            var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            var ipEndPoint = new IPEndPoint(ipAddress, _port);
            Console.WriteLine($"Client will connect..");
            await _tcpClient.ConnectAsync(ipEndPoint);
            _protocol = new Protocol(_tcpClient.GetStream());
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
                        Console.WriteLine($"Client [{_name}] message received: {message}");
                    }
                }
                while (messages.Count > 0 && !messages.Contains("close connection"));
                await StartSending();
            });
        }

        public async Task StartSending()
        {
            await Task.Run(async () =>
            {
                await SendMessage("Ok, as you will! :(");
                await SendMessage("close connection");
            });
        }

        private async Task SendMessage(string message)
        {
            await _protocol.SendMessage(message);

            Console.WriteLine($"Client [{_name}] message sent: {message}");
        }


        /*private async Task<string> ReadMessage(NetworkStream stream)
        {
            string message = "";
            var buffer = new byte[10];
            int received = 0;
            do
            {
                received = await stream.ReadAsync(buffer);
                message += Encoding.UTF8.GetString(buffer, 0, received);
            }
            while (received == buffer.Length);

            return message;
        }*/
    }
}
