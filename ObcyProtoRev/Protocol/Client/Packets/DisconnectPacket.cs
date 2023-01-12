using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a disconnect request packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class DisconnectPacket : Packet
    {
        /// <summary>
        /// Creates a new instance of DisconnectPacket class.
        /// </summary>
        /// <param name="strangerUID">UID of a currently connected stranger.</param>
        public DisconnectPacket(string strangerUID)
        {
            Header = "_distalk";

            Data = new JObject();
            Data["ckey"] = strangerUID;

            Data["ceid"] = Connection.ActionID;
        }
    }
}
