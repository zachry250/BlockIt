using BlockIt.P2PNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlockIt.NetworkCommunication
{
    public static class Extensions
    {
        public static async Task Send<T>(this Connection connection, T data) where T : BaseNetworkMessage
        {
            await connection.Send<T>(JsonSerializer.Serialize(data));
        }

        public static async Task Send<T>(this Connection connection, string message = "") where T : BaseNetworkMessage
        {
            await connection.SendMessage($"{BaseNetworkMessage.ToCommand<T>()}{message}");
        }

        public static bool IsCommand<T>(this string message) where T : BaseNetworkMessage
        {
            return message.StartsWith(BaseNetworkMessage.ToCommand<T>());
        }

        public static string Data<T>(this string message) where T : BaseNetworkMessage
        {
            return message.Replace(BaseNetworkMessage.ToCommand<T>(), "");
        }

        public static T? Deserialize<T>(this string message) where T : BaseNetworkMessage
        {
            return JsonSerializer.Deserialize<T>(message.Data<T>());
        }
    }

    public class BaseNetworkMessage
    {
        public static string ToCommand<T>() where T : BaseNetworkMessage
        {
            return $"[%{typeof(T).Name}%]";
        }
    }

    public class AddMessage : BaseNetworkMessage
    {
        public long Timestamp { get; set; }
        public string Message { get; set; }
    }

    public class GetBlocks : BaseNetworkMessage;

    public class GetBlocksResponse : BaseNetworkMessage;

    public class GetAvailableConnection : BaseNetworkMessage
    {
        public Guid ConnectionIdToConnect { get; set; }
    }

    public class GetAvailableConnectionResponse : BaseNetworkMessage
    {
        public Guid ConnectionIdToConnect { get; set; }
        public ConnectionInfo? ConnectionInfo { get; set; }
    }

    public class ConnectToNode : BaseNetworkMessage
    {
        public ConnectionInfo? ConnectionInfo { get; set; }
    }
}
