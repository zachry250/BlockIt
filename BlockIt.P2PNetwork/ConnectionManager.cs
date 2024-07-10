using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlockIt.P2PNetwork
{
    public class ConnectionManager
    {
        private List<Connection> _connections = new List<Connection>();
        private string _name;
        private readonly Func<Connection, string, Task> _messageListener;
        private readonly Func<Connection, Task> _clientConnected;

        public List<Connection> Connections => _connections;

        public ConnectionManager(
            string name, 
            Func<Connection, string, Task> messageListener,
            Func<Connection, Task> clientConnected)
        {
            _name = name;
            _messageListener = messageListener;
            _clientConnected = clientConnected;
        }

        public async Task StartServer(Server server, int port)
        {
            await Task.Run(async () =>
            {
                await server.Listen(port, ServerConnectionCreated);
            });
        }

        private async Task ServerConnectionCreated(Connection connection)
        {
            Console.WriteLine($"Server {_name}: Client connected");
            _connections.Add(connection);
            connection.RegisterMessageListener(_messageListener);
            await _clientConnected(connection);
        }

        public async Task ConnectToServer(Client client, IPAddress ipAdress, int port)
        {
            await Task.Run(async () =>
            {
                await client.Connect(ipAdress, port, ClientConnectionCreated);
            });
        }

        private async Task ClientConnectionCreated(Connection connection)
        {
            Console.WriteLine($"New client connected! - {connection.Name}");
            _connections.Add(connection);
            connection.RegisterMessageListener(_messageListener);
        }


        
    }
}
