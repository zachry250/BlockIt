using BlockIt.Core;
using BlockIt.NetworkCommunication;
using BlockIt.NetworkManager;
using BlockIt.NetworkNode;
using BlockIt.P2PNetwork;
using System.Net;

namespace BlockIt.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager("Network Manager 1", new List<int> { 4042, 4043, 4044 });
            manager.Start();

            var node1 = new Node(new NodeState("Node 1", new List<int> { 4142, 4143 }));
            node1.ConnectTo(new ConnectionInfo("127.0.0.1", 4042));

            var node2 = new Node(new NodeState("Node 2", new List<int> { 4242, 4243 }));
            node2.ConnectTo(new ConnectionInfo("127.0.0.1", 4043));

            var node3 = new Node(new NodeState("Node 3", new List<int> { 4342, 4343 }));
            node3.ConnectTo(new ConnectionInfo("127.0.0.1", 4044));

            Thread.Sleep(500);
            manager.AddMessage("1 add testing block added! :)").Wait();
            manager.AddMessage("2 add, will be fun!").Wait();
            manager.AddMessage("3 add, will be even more fun!!!!").Wait();
            manager.AddMessage("4 how good will this work?").Wait();
            manager.AddMessage("5 I hope really good hehe.").Wait();
            Thread.Sleep(1000);
            manager.ReturnBlocks().Wait();
            Thread.Sleep(1000);
        }
    }
}
