using BlockIt.Core;
using BlockIt.P2PNetwork;

namespace BlockIt.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int numberOfConnections = 1;
            List<Task> tasks = new List<Task>();
            List<Server> servers = new List<Server>();
            List<Client> clients = new List<Client>();
            
            for (int i = 0; i < numberOfConnections; i++)
            {
                Server server = new Server($"Server {i}", 4240+i);
                servers.Add(server);
                tasks.Add(RunServer(server));
                
                Client client = new Client($"Client {i}", 4240+i);
                clients.Add(client);
                tasks.Add(RunClient(client));
            }



            Task.WaitAll(tasks.ToArray());


            var stringBlockchain = new StringBlockchain();
            Console.WriteLine(stringBlockchain.PrintBlock(0));
            stringBlockchain.Add("first testing :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(1));
            stringBlockchain.Add("second testing :) :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(2));
        }

        private static async Task RunServer(Server server)
        {
            await Task.Run(async () =>
            {
                await server.Listen();
                _ = server.StartReading();
                await server.StartSending();
            });
        }

        private static async Task RunClient(Client client)
        {
            await Task.Run(async () =>
            {
                await client.Connect();
                await client.StartReading();
            });
        }
    }
}
