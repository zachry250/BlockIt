using BlockIt.NetworkCommunication;
using BlockIt.P2PNetwork;
using System.Text.Json;

namespace BlockIt.NetworkManager
{
    public class Manager : MessageVisitor
    {
        private readonly string? _name;
        private readonly Server _server;
        private readonly List<int> _serverPorts;
        private ConnectionManager _connectionManager;

        public Manager(string name, List<int> serverPorts)
        {
            _name = name;
            _server = new Server($"{name}_server");
            _serverPorts = serverPorts;
            _connectionManager = new ConnectionManager(name, MessageListener, ClientConnected);
        }   

        public void Start()
        {
            foreach (var port in _serverPorts)
            {
                _ = _connectionManager.StartServer(_server, port);
            }
        }

        public async Task Stop()
        {
            foreach (var connection in _connectionManager.Connections)
            {
                connection.Close();
            }
        }

        public async Task AddMessage(string message)
        {
            var addMessage = new AddMessage();
            addMessage.Timestamp = DateTime.UtcNow.Ticks;
            addMessage.Message = message;

            /*foreach (var connection in _connectionManager.Connections)
            {
                await connection.Send(addMessage);
            }*/
            var random = new Random();
            var index = random.Next(_connectionManager.Connections.Count);
            var connection = _connectionManager.Connections[index];
            await connection.Send(addMessage);
        }

        public async Task ReturnBlocks()
        {
            foreach (var connection in _connectionManager.Connections)
            {
                await connection.Send<GetBlocks>();
            }
        }

        private async Task MessageListener(Connection connection, string message)
        {
            var messageInstance = message.Deserialize();
            await messageInstance.RouteMessage(connection, this);
        }

        public override async Task ProcessMessage(Connection connection, GetBlocksResponse message)
        {
            Console.WriteLine(message.Response);
        }

        public override async Task ProcessMessage(Connection connection, GetAvailableConnectionResponse message)
        {
            var connectedConnection = _connectionManager.Connections.FirstOrDefault(x => x.Id == message.ConnectionIdToConnect);
            var connectToNode = new ConnectToNode();
            connectToNode.ConnectionInfo = message.ConnectionInfo;
            connectedConnection.Send(connectToNode);
        }

        private async Task ClientConnected(Connection connection)
        {
            var connectionToConnectTo = _connectionManager.Connections.FirstOrDefault(x => x.Id != connection.Id);
            if (connectionToConnectTo != null)
            {
                var getAvailableConnection = new GetAvailableConnection();
                getAvailableConnection.ConnectionIdToConnect = connection.Id;
                await connectionToConnectTo.Send(getAvailableConnection);
            }
        }
    }
}
