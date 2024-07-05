using BlockIt.NetworkCommunication;
using BlockIt.P2PNetwork;

namespace BlockIt.NetworkManager
{
    public class Manager
    {
        private readonly string? _name;
        private readonly Server _server;
        private readonly List<int> _serverPorts;
        private List<Connection> _connections;

        public Manager(string name, List<int> serverPorts)
        {
            _name = name;
            _server = new Server($"{name}_server");
            _serverPorts = serverPorts;
            _connections = new List<Connection>();
        }   

        public void Start()
        {
            foreach ( var port in _serverPorts )
            {
                _ = RunServer(port);
            }
        }

        public async Task Stop()
        {
            foreach (var connection in _connections)
            {
                connection.Close();
            }
        }

        public async Task AddMessage(string message)
        {
            foreach (var connection in _connections)
            {
                await connection.Send<AddMessage>(message);
            }
        }

        public async Task ReturnBlocks()
        {
            foreach (var connection in _connections)
            {
                await connection.Send<GetBlocks>();
            }
        }

        private async Task RunServer(int port)
        {
            await Task.Run(async () =>
            {
                await _server.Listen(port, NewServerConnectionCreated);
            });
        }

        private async Task NewServerConnectionCreated(Connection connection)
        {
            Console.WriteLine($"{_name}: Client connected");
            _connections.Add(connection);
            connection.RegisterMessageListener(MessageListener);
        }

        private async Task MessageListener(Connection connection, string message)
        {
            if(message.IsCommand<GetBlocksResponse>())
            {
                var getBlocksResponse = message.Data<GetBlocksResponse>();
                Console.WriteLine(getBlocksResponse);
            }
        }
    }
}
