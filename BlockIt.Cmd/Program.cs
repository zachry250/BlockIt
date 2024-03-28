using BlockIt.Core;
using BlockIt.NetworkNode;
using BlockIt.P2PNetwork;
using System.Net;

namespace BlockIt.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cci = new ConnectionInfo(IPAddress.Parse("127.0.0.1"), 12345);

            Console.WriteLine(cci);


            int numberOfConnections = 1;
            List<Task> activeClients = new List<Task>();
            Server server;
            List<Client> clients = new List<Client>();

            server = new Server($"Server");
            _ = RunServer(server, 4240);

            Thread.Sleep(3000);
            
            for (int i = 0; i < numberOfConnections; i++)
            {
                Client client = new Client($"Client {i}");
                clients.Add(client);
                activeClients.Add(RunClient(client, 4240));
            }
            Task.WaitAll(activeClients.ToArray());
            server.Close();

            var stringBlockchain = new StringBlockchain();
            Console.WriteLine(stringBlockchain.PrintBlock(0));
            stringBlockchain.Add("first testing :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(1));
            stringBlockchain.Add("second testing :) :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(2));
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
