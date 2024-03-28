using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace BlockIt.P2PNetwork
{
    public class Client: ITCPConnection
    {
        private string? _name;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        public Client(string name)
        {
            _name = name;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public async Task Connect(IPAddress ipAddress, int port, Func<Connection, Task> newConnectionCreated)
        {
            var tcpClient = new TcpClient();
            var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            Console.WriteLine($"Client will connect..");
            await tcpClient.ConnectAsync(ipEndPoint);
            await AcceptConnection(tcpClient, newConnectionCreated);
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
                    connection.RegisterMessageReceiver().Wait(_cancellationToken);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
