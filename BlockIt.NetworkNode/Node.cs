using BlockIt.Core;
using BlockIt.NetworkCommunication;
using BlockIt.P2PNetwork;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace BlockIt.NetworkNode
{
    public class Node
    {
        private NodeState _nodeState;
        private NodeCommunication _nodeCommunication;
        private ConnectionManager _connectionManager;

        public Node(NodeState nodeState)
        {
            _nodeState = nodeState;
            _nodeCommunication = new NodeCommunication(_nodeState, ConnectTo, StartServer, GetConnections);
            _connectionManager = new ConnectionManager(_nodeState.Name, _nodeCommunication.Listener, ClientConnected);
        }

        public async Task ConnectTo(ConnectionInfo connectionInfo)
        {
            if (connectionInfo != null)
            {
                Client client = new Client($"Network Node {_nodeState.Name}");
                await _connectionManager.ConnectToServer(client, connectionInfo.GetIPAddress(), connectionInfo.Port);
            }
        }

        private async Task StartServer(int port)
        {
            Console.WriteLine($"{_nodeState.Name} - Start server");
            _ = _connectionManager.StartServer(_nodeState.NodeServer, port);
        }

        private async Task ClientConnected(Connection connection)
        {

        }

        private List<Connection> GetConnections()
        {
            return _connectionManager.Connections;
        }
    }
}
