using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.NetworkCommunication
{
    public class ConnectionInfo
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public ConnectionInfo(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public IPAddress GetIPAddress()
        {
            return IPAddress.Parse(IpAddress);
        }

    }
}
