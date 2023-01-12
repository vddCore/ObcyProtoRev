using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a chatstate packet which is ready to be sent as-is.
    /// </summary>
    public sealed class ChatstatePacket : Packet
    {
        /// <summary>
        /// Creates a new instance of ChatstatePacket class.
        /// </summary>
        /// <param name="typing">A value indicating whether or not this packet means that client is typing.</param>
        /// <param name="strangerUID">A value describing target stranger UID.</param>
        public ChatstatePacket(bool typing, string strangerUID)
        {
            Header = "_mtyp";

            Data = new JObject();
            Data["ckey"] = strangerUID;
            Data["val"] = typing;
        }
    }
}
