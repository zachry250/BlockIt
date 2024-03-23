using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.P2PNetwork
{
    public class Message
    {
        public string Header { get; private set; }

        public string Body { get; private set; }

        public Message(string header, string body) 
        {
            Header = header;
            Body = body;

        }
    }
}
