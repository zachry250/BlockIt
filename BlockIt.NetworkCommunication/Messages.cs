using BlockIt.P2PNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlockIt.NetworkCommunication
{
    public static class Extensions
    {
        public static async Task Send<T>(this Connection connection, string message = "") where T : Message
        {
            await connection.SendMessage($"{Message.ToCommand<T>()}{message}");
        }

        public static bool IsCommand<T>(this string message) where T : Message
        {
            return message.StartsWith(Message.ToCommand<T>());
        }

        public static string Data<T>(this string message) where T : Message
        {
            return message.Replace(Message.ToCommand<T>(), "");
        }
    }

    public class Message
    {
        public static string ToCommand<T>() where T : Message
        {
            return $"[%{typeof(T).Name}%]";
        }
    }

    public class AddMessage : Message;

    public class GetBlocks : Message;

    public class GetBlocksResponse : Message;

    public class ConnectToNode : Message;
}
