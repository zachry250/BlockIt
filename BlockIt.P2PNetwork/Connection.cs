using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.P2PNetwork
{
    public class Connection : IDisposable
    {
        private readonly string _name;
        private readonly ITCPConnection _tcpConnection;
        private readonly Protocol _protocol;
        private Func<Connection, string, Task>? _messageListener;

        public string Name { get { return _name; } }

        public bool IsServer { get { return _tcpConnection is Server; } }

        public Connection(string name, ITCPConnection tcpConnection, Protocol protocol)
        {
            _name = name;
            _tcpConnection = tcpConnection;
            _protocol = protocol;
            _messageListener = null;
        }

        public void Dispose()
        {
            _protocol.Dispose();
        }

        public void Close()
        {
            _tcpConnection.Close();
        }

        public async Task RegisterMessageReceiver()
        {
            await Task.Run(async () =>
            {
                List<string> messages = null;
                do
                {
                    messages = await _protocol.ReadMessages();
                    foreach (var message in messages)
                    {
                        await MessageReceived(message);
                    }
                }
                while (messages.Count > 0 && !messages.Contains("close connection"));
            });
        }

        public void RegisterMessageListener(Func<Connection, string, Task> messageListener)
        {
            _messageListener = messageListener;
        }

        private async Task MessageReceived(string message)
        {
            //Console.WriteLine($"{_name} - {message}");
            if ( _messageListener != null )
            {
                await _messageListener(this, message);
            }
        }

        public async Task SendMessage(string message)
        {
            //Console.WriteLine($"{_name} - {message}");
            await _protocol.SendMessage(message);
        }
    }
}
