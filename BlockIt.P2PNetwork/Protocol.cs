using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.P2PNetwork
{
    public class Protocol : IDisposable
    {
        private string _start = "[%START%]";
        private string _end = "[%END%]";
        private NetworkStream _stream;

        public Protocol(NetworkStream stream) 
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public async Task SendMessage(string message)
        {
            var envelopedMessage = $"{_start}{message}{_end}";
            var envelopedMessageBytes = Encoding.UTF8.GetBytes(envelopedMessage);


            await _stream.WriteAsync(envelopedMessageBytes);
        }

        public async Task<List<string>> ReadMessages()
        {
            string message = "";
            var buffer = new byte[10];
            int received = 0;
            do
            {
                received = await _stream.ReadAsync(buffer);
                message += Encoding.UTF8.GetString(buffer, 0, received);
            }
            while (received == buffer.Length);

            var splittedMessages = message.Split($"{_end}").Select(x => x.Replace(_start, "")).Where(x => x.Length > 0).ToList();

            return splittedMessages;
        }
    }
}
