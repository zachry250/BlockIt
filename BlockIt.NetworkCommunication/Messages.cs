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

        public static string? Data<T>(this string message) where T : BaseNetworkMessage
        {
            var result = message.Replace(BaseNetworkMessage.ToCommand<T>(), "");
            if (string.IsNullOrWhiteSpace(result))
            {
                return null;
            }
            return result;
        }

        public static T? Deserialize<T>(this string message) where T : BaseNetworkMessage, new()
        {
            var data = message.Data<T>();
            if (data == null)
            {
                return new T();
            }
            return JsonSerializer.Deserialize<T>(data);
        }

        public static bool TryDeserialize<T>(this string message, out BaseNetworkMessage? response) where T : BaseNetworkMessage, new()
        {
            if(message.IsCommand<T>())
            {
                response = message.Deserialize<T>();
                return true;
            }
            response = null;
            return false;
        }

        public static BaseNetworkMessage Deserialize(this string message)
        {
            BaseNetworkMessage result;
            if (message.TryDeserialize<AddMessage>(out result)) return result;
            if (message.TryDeserialize<GetBlocks>(out result)) return result;
            if (message.TryDeserialize<GetBlocksResponse>(out result)) return result;
            if (message.TryDeserialize<GetAvailableConnection>(out result)) return result;
            if (message.TryDeserialize<GetAvailableConnectionResponse>(out result)) return result;
            if (message.TryDeserialize<ConnectToNode>(out result)) return result;
            else return new BaseNetworkMessage();
        }
    }


    public class BaseNetworkMessage
    {
        public static string ToCommand<T>() where T : BaseNetworkMessage
        {
            return $"[%{typeof(T).Name}%]";
        }

        public virtual async Task RouteMessage(Connection connection, MessageVisitor visitor) { await new Task(() => { }); }
    }

    public class AddMessage : BaseNetworkMessage
    {
        public long Timestamp { get; set; }
        public string Message { get; set; }
        public override async Task RouteMessage(Connection connection, MessageVisitor visitor)
        {
            await visitor.ProcessMessage(connection, this);
        }
    }

    public class GetBlocks : BaseNetworkMessage
    {
        public override async Task RouteMessage(Connection connection, MessageVisitor visitor)
        {
            await visitor.ProcessMessage(connection, this);
        }
    }

    public class GetBlocksResponse : BaseNetworkMessage
    {
        public string? Response { get; set; }
        public override async Task RouteMessage(Connection connection, MessageVisitor visitor)
        {
            await visitor.ProcessMessage(connection, this);
        }
    }

    public class GetAvailableConnection : BaseNetworkMessage
    {
        public Guid ConnectionIdToConnect { get; set; }
        public override async Task RouteMessage(Connection connection, MessageVisitor visitor)
        {
            await visitor.ProcessMessage(connection, this);
        }
    }

    public class GetAvailableConnectionResponse : BaseNetworkMessage
    {
        public Guid ConnectionIdToConnect { get; set; }
        public ConnectionInfo? ConnectionInfo { get; set; }
        public override async Task RouteMessage(Connection connection, MessageVisitor visitor)
        {
            await visitor.ProcessMessage(connection, this);
        }
    }

    public class ConnectToNode : BaseNetworkMessage
    {
        public ConnectionInfo? ConnectionInfo { get; set; }
        public override async Task RouteMessage(Connection connection, MessageVisitor visitor)
        {
            await visitor.ProcessMessage(connection, this);
        }
    }
}
