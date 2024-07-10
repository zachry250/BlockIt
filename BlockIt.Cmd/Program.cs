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
            //Thread.Sleep(100);
            manager.AddMessage("2 add, will be fun!").Wait();
            //Thread.Sleep(100);
            manager.AddMessage("3 add, will be even more fun!!!!").Wait();
            //Thread.Sleep(100);
            manager.AddMessage("4 how good will this work?").Wait();
            //Thread.Sleep(100);
            manager.AddMessage("5 I hope really good hehe.").Wait();
            //Thread.Sleep(1000);
            //Thread.Sleep(7000);
            Thread.Sleep(500);
            manager.ReturnBlocks().Wait();
            Thread.Sleep(1000);
            
            //manager.Stop();

/*            int numberOfConnections = 2;
            List<Task> activeClients = new List<Task>();
            Server server;
            //List<Client> clients = new List<Client>();

            server = new Server($"Server");
            _ = RunServer(server, 4240);

            //Thread.Sleep(3000);
            
            for (int i = 0; i < numberOfConnections; i++)
            {
                Client client = new Client($"Client {i}");
                //clients.Add(client);
                activeClients.Add(RunClient(client, 4240));
            }
            Task.WaitAll(activeClients.ToArray());
            server.Close();
*/

            /*var stringBlockchain = new StringBlockchain();
            Console.WriteLine(stringBlockchain.PrintBlock(0));
            stringBlockchain.Add("first testing :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(1));
            stringBlockchain.Add("second testing :) :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(2));
            */
        }

        private static async Task RunServer(Server server, int port)
        {
            await Task.Run(async () =>
            {
                await server.Listen(port, NewConnectionCreated);
                //_ = server.StartReading();
                //await server.StartSending();
            });
        }

        private static async Task NewConnectionCreated(Connection connection)
        {
            Console.WriteLine($"New connection created! - {connection.Name}");
            connection.RegisterMessageListener(MessageListener);
            if(!connection.IsServer)
            {
                await connection.SendMessage($"Hi master! I am {connection.Name}, who are you?");
            }
        }

        private static async Task MessageListener(Connection connection, string message)
        {
            if (message.Contains("who are you")) 
            {
                await connection.SendMessage($"Nice to meet you. I am {connection.Name}");
            }
            if (message.Contains("Nice to meet you"))
            {
                await connection.SendMessage($"Oki, lets close");
                //await connection.SendMessage($"close connection");
                connection.Close();
            }
        }

        private static async Task RunClient(Client client, int port)
        {
            await Task.Run(async () =>
            {
                await client.Connect(IPAddress.Parse("127.0.0.1"), port, NewConnectionCreated);
                //await client.StartReading();
            });
        }
    }
}
