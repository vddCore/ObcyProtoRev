using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a random topic request packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class RandomTopicPacket : Packet
    {
        /// <summary>
        /// Creates a new instance of RandomTopicPacket class.
        /// </summary>
        /// <param name="strangerUID">UID of a currently connected stranger.</param>
        public RandomTopicPacket(string strangerUID)
        {
            Header = "_randtopic";

            Data = new JObject();
            Data["ckey"] = strangerUID;
        }
    }
}
