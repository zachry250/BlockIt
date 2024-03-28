using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.NetworkNode
{
    public class ConnectionInfo
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;

        public IPAddress IPAddress => _ipAddress;

        public int Port => _port;

        public ConnectionInfo(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public static ConnectionInfo? Parse(string connectionString)
        {
            // Format = "[%IP%]<IP Address>,[%PORT%]<Port number>"
            ConnectionInfo? result = null;
            try
            {
                var splittedConnectionString = connectionString.Split(",");
                if (splittedConnectionString.Length > 0)
                {
                    var ipStringSection = splittedConnectionString[0];
                    var portStringSection = splittedConnectionString[1];
                    if (ipStringSection.StartsWith("[%IP%]") && portStringSection.StartsWith("[%PORT%]"))
                    {
                        var ipString = ipStringSection.Replace("[%IP%]", "");
                        var portString = ipStringSection.Replace("[%PORT%]", "");
                        result = new ConnectionInfo(IPAddress.Parse("127.0.0.1"),int.Parse(portString));
                    }
                }
            }
            catch 
            {
                Console.WriteLine($"Unable to parse connectionString: {connectionString}");
            }
            return result;
        }

        public override string ToString()
        {
            var result = $"[%IP%]{_ipAddress},[%PORT%]{_port}";
            return result;
        }
    }
}
