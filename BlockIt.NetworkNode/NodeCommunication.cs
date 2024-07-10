using BlockIt.Core;
using BlockIt.NetworkCommunication;
using BlockIt.P2PNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BlockIt.NetworkNode
{
    internal class NodeCommunication
    {
        private NodeState _nodeState;
        private Func<ConnectionInfo, Task> _connectTo;
        private Func<int, Task> _startServer;
        private Func<List<Connection>> _getConnections;
        private Object _lock = new Object();

        public NodeCommunication(NodeState nodeState, 
            Func<ConnectionInfo, Task> connectTo, 
            Func<int, Task> startServer,
            Func<List<Connection>> getConnections)
        {
            _nodeState = nodeState;
            _connectTo = connectTo;
            _startServer = startServer;
            _getConnections = getConnections;
        }

        public async Task Listener(Connection connection, string message)
        {
            if (message.IsCommand<ConnectToNode>())
            {
                var connectToNode = message.Deserialize<ConnectToNode>();
                var connectionInfo = connectToNode?.ConnectionInfo;
                if (connectionInfo != null)
                {
                    _connectTo(connectionInfo);
                    //Client client = new Client($"Network Node - {connectionInfo.GetIPAddress()}:{connectionInfo.Port}");
                    //await client.Connect(connectionInfo.IPAddress, connectionInfo.Port, NewNetworkNodeConnected);
                    //_networkNodeClients.Add(client);
                }
            }
            else if (message.IsCommand<GetAvailableConnection>())
            {
                var port = _nodeState.GetFreeServerPort();
                var input = message.Deserialize<GetAvailableConnection>();
                if (port.HasValue && input != null)
                {
                    await _startServer(port.Value);
                    var response = new GetAvailableConnectionResponse();
                    response.ConnectionIdToConnect = input.ConnectionIdToConnect;
                    response.ConnectionInfo = new ConnectionInfo("127.0.0.1", port.Value);
                    await connection.Send(response);
                }
            }
            else if (message.IsCommand<AddMessage>())
            {
                lock (_lock)
                {
                    var addMessage = message.Deserialize<AddMessage>();
                    var newBlock = _nodeState.StringBlockchain.CreateBlock(addMessage.Timestamp, addMessage.Message);
                    if (!_nodeState.StringBlockchain.HasBlock(newBlock))
                    {
                        Console.WriteLine($"{_nodeState.Name} adds block {addMessage.Message}");
                        _nodeState.StringBlockchain.Add(newBlock);
                        _ = Broadcast(addMessage);
                    }
                    else
                    {
                        Console.WriteLine($"{_nodeState.Name} has block {addMessage.Message}");
                        _nodeState.StringBlockchain.Sort();
                    }
                    
                }
            }
            else if (message.IsCommand<GetBlocks>())
            {
                StringBuilder returnedBlocks = new StringBuilder();
                returnedBlocks.AppendLine($"Node name: {_nodeState.Name}");
                for (int i = 0; i < _nodeState.StringBlockchain.Count; i++)
                {
                    returnedBlocks.Append(_nodeState.StringBlockchain.PrintBlock(i));
                }
                await connection.Send<GetBlocksResponse>(returnedBlocks.ToString());
            }
        }

        private async Task Broadcast(AddMessage message)
        {
            foreach (var connection in _getConnections())
            {
                await connection.Send(message);
            }
        }
    }
}
