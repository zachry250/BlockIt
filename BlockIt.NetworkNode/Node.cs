using BlockIt.Core;
using BlockIt.NetworkCommunication;
using BlockIt.P2PNetwork;
using System.Net;
using System.Text;

namespace BlockIt.NetworkNode
{
    public class Node
    {
        private readonly string? _name;
        private readonly List<Client> _networkManagerClients;
        private readonly List<Client> _networkNodeClients;
        private Server _networkServer;
        private StringBlockchain _stringBlockchain;


        public Node(string name)
        {
            _name = name;
            _networkManagerClients = new List<Client>();
            _networkNodeClients = new List<Client>();
            _stringBlockchain = new StringBlockchain();
        }

        public async Task ConnectToNetworkManager(ConnectionInfo connectionInfo)
        {
            if (connectionInfo != null)
            {
                Client client = new Client($"Network Node {_name} - {connectionInfo.IPAddress}:{connectionInfo.Port}");
                //await client.Connect(connectionInfo.IPAddress, connectionInfo.Port, NewNetworkManagerConnected);
                //_networkManagerClients.Add(client);
                await RunManagerClient(client, connectionInfo);
            }
        }

        private async Task RunManagerClient(Client client, ConnectionInfo connectionInfo)
        {
            await Task.Run(async () =>
            {
                await client.Connect(connectionInfo.IPAddress, connectionInfo.Port, NewNetworkManagerConnected);
                //await client.StartReading();
            });
        }

        private async Task NewNetworkManagerConnected(Connection connection)
        {
            Console.WriteLine($"New network manager connected! - {connection.Name}");
            connection.RegisterMessageListener(NetworkManagerListener);
        }

        private async Task NetworkManagerListener(Connection connection, string message)
        {
            if (message.IsCommand<ConnectToNode>())
            {
                var connectionString = message.Data<ConnectToNode>();
                var connectionInfo = ConnectionInfo.Parse(connectionString);
                if (connectionInfo != null)
                {
                    Client client = new Client($"Network Node - {connectionInfo.IPAddress}:{connectionInfo.Port}");
                    await client.Connect(connectionInfo.IPAddress, connectionInfo.Port, NewNetworkNodeConnected);
                    _networkNodeClients.Add(client);
                }
            }
            else if (message.IsCommand<AddMessage>())
            {
                var messageToAdd = message.Data<AddMessage>();
                _stringBlockchain.Add(messageToAdd);
            }
            else if (message.IsCommand<GetBlocks>())
            {
                StringBuilder returnedBlocks = new StringBuilder();
                //returnedBlocks.AppendLine("[%ReturnedBlocks%]");
                returnedBlocks.AppendLine($"Node name: {_name}");
                for(int i = 0;  i < _stringBlockchain.Count; i++)
                {
                    returnedBlocks.Append(_stringBlockchain.PrintBlock(i));
                }
                await connection.Send<GetBlocksResponse>(returnedBlocks.ToString());
                //await connection.SendMessage(returnedBlocks.ToString());
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
