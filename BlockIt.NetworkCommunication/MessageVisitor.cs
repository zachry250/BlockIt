using BlockIt.P2PNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.NetworkCommunication
{
    public class MessageVisitor
    {
        public virtual async Task ProcessMessage(Connection connection, AddMessage message) { }
        public virtual async Task ProcessMessage(Connection connection, GetBlocks message) { }
        public virtual async Task ProcessMessage(Connection connection, GetBlocksResponse message) { }
        public virtual async Task ProcessMessage(Connection connection, GetAvailableConnection message) { }
        public virtual async Task ProcessMessage(Connection connection, GetAvailableConnectionResponse message) { }
        public virtual async Task ProcessMessage(Connection connection, ConnectToNode message) { }
    }
}
