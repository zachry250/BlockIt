using BlockIt.Core;
using BlockIt.P2PNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlockIt.NetworkNode
{
    public class NodeState
    {
        public string Name { get; private set; }
        public Server NodeServer { get; private set; }
        public List<int> NodeServerPorts { get; private set; }
        public List<int> UsedNodeServerPorts { get; private set; }
        public StringBlockchain StringBlockchain { get; private set; }
        

        public NodeState(string name, List<int> nodeServerPorts) 
        {
            Name = name;
            NodeServer = new Server($"{name}_node_server");
            NodeServerPorts = nodeServerPorts;
            UsedNodeServerPorts = new List<int>();
            StringBlockchain = new StringBlockchain();
        }

        public int? GetFreeServerPort()
        {
            int? result = null;
            foreach (int port in NodeServerPorts)
            {
                if (!UsedNodeServerPorts.Any(p => p == port))
                {
                    result = port;
                    UsedNodeServerPorts.Add(port);
                    break;
                }
            }
            return result;
        }
    }
}
