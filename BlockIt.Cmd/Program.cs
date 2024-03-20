using BlockIt.Core;

namespace BlockIt.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var stringBlockchain = new StringBlockchain();
            Console.WriteLine(stringBlockchain.PrintBlock(0));
            stringBlockchain.Add("first testing :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(1));
            stringBlockchain.Add("second testing :) :) ");
            Console.WriteLine(stringBlockchain.PrintBlock(2));

        }
    }
}
