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
    internal class NodeCommunication : MessageVisitor
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
            var messageInstance = message.Deserialize();
            await messageInstance.RouteMessage(connection, this);
        }

        public override async Task ProcessMessage(Connection connection, AddMessage message)
        {
            lock (_lock)
            {
                var newBlock = _nodeState.StringBlockchain.CreateBlock(message.Timestamp, message.Message);
                if (!_nodeState.StringBlockchain.HasBlock(newBlock))
                {
                    Console.WriteLine($"{_nodeState.Name} adds block {message.Message}");
                    _nodeState.StringBlockchain.Add(newBlock);
                    _ = Broadcast(message);
                }
                else
                {
                    Console.WriteLine($"{_nodeState.Name} has block {message.Message}");
                }
                _nodeState.StringBlockchain.Sort();
            }
        }

        public override async Task ProcessMessage(Connection connection, GetBlocks message)
        {
            StringBuilder returnedBlocks = new StringBuilder();
            returnedBlocks.AppendLine($"Node name: {_nodeState.Name}");
            for (int i = 0; i < _nodeState.StringBlockchain.Count; i++)
            {
                returnedBlocks.Append(_nodeState.StringBlockchain.PrintBlock(i));
            }
            var response = new GetBlocksResponse();
            response.Response = returnedBlocks.ToString();
            await connection.Send(response);
        }

        public override async Task ProcessMessage(Connection connection, GetAvailableConnection message)
        {
            var port = _nodeState.GetFreeServerPort();
            if (port.HasValue && message != null)
            {
                await _startServer(port.Value);
                var response = new GetAvailableConnectionResponse();
                response.ConnectionIdToConnect = message.ConnectionIdToConnect;
                response.ConnectionInfo = new ConnectionInfo("127.0.0.1", port.Value);
                await connection.Send(response);
            }
        }

        public override async Task ProcessMessage(Connection connection, ConnectToNode message)
        {
            var connectionInfo = message?.ConnectionInfo;
            if (connectionInfo != null)
            {
                _connectTo(connectionInfo);
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
