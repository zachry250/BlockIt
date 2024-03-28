using BlockIt.P2PNetwork;
using System.Net;

namespace BlockIt.NetworkNode
{
    public class NetworkNode
    {
        private readonly string? _name;
        private readonly List<Client> _networkManagerClients;
        private readonly List<Client> _networkNodeClients;
        private Server _networkServer;


        public NetworkNode(string name)
        {
            _name = name;
            _networkManagerClients = new List<Client>();
            _networkNodeClients = new List<Client>();
        }

        public async Task ConnectToNetworkManager(ConnectionInfo connectionInfo)
        {
            if (connectionInfo != null)
            {
                Client client = new Client($"Network Node - {connectionInfo.IPAddress}:{connectionInfo.Port}");
                await client.Connect(connectionInfo.IPAddress, connectionInfo.Port, NewNetworkManagerConnected);
                _networkManagerClients.Add(client);
            }
        }

        private async Task NewNetworkManagerConnected(Connection connection)
        {
            Console.WriteLine($"New network manager connected! - {connection.Name}");
            connection.RegisterMessageListener(NetworkManagerListener);
        }

        private async Task NetworkManagerListener(Connection connection, string message)
        {
            if (message.StartsWith("[%ConnectToNode%]"))
            {
                var connectionString = message.Replace("[%ConnectToNode%]", "");
                var connectionInfo = ConnectionInfo.Parse(connectionString);
                if (connectionInfo != null)
                {
                    Client client = new Client($"Network Node - {connectionInfo.IPAddress}:{connectionInfo.Port}");
                    await client.Connect(connectionInfo.IPAddress, connectionInfo.Port, NewNetworkNodeConnected);
                    _networkNodeClients.Add(client);
                }
            }
        }

        private async Task NewNetworkNodeConnected(Connection connection)
        {
            Console.WriteLine($"New network node connected! - {connection.Name}");
            connection.RegisterMessageListener(NetworkNodeListener);
        }

        private async Task NetworkNodeListener(Connection connection, string message)
        {

        }



        public async Task Run(IPAddress nodeIpAddress, int port)
        {
            await Task.Run(async () =>
            {
                //await _client.Connect(nodeIpAddress, port);
                //await _client.RegisterMessageReceiver(MessageReceiver);
            });
        }

        private async Task MessageReceiver(string message)
        {
            Console.WriteLine($"[{_name}] message received: {message}");
        }

        public async Task SendMessage(string message)
        {
            //await _client.SendMessage(message);

            Console.WriteLine($"[{_name}] message sent: {message}");
        }
    }
}
