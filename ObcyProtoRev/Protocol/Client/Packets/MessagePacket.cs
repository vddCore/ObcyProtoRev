using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a message packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class MessagePacket : Packet
    {
        /// <summary>
        /// Creates a new instance of MessagePacket class.
        /// </summary>
        /// <param name="body">Message content.</param>
        /// <param name="strangerUID">UID of a stranger who will receive this message.</param>
        public MessagePacket(string body, string strangerUID)
        {
            Header = "_pmsg";

            Data = new JObject();
            Data["ckey"] = strangerUID;
            Data["msg"] = body;

            Data["ceid"] = Connection.ActionID;
        } 
    }
}
