using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a client information packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class ClientInfoPacket : Packet
    {
        /// <summary>
        /// Creates a new instance of ClientInfoPacket class.
        /// </summary>
        /// <param name="isMobile">A value indicating whether or not this packet is sent from a mobile device.</param>
        /// <param name="userAgent">A value describing application's identity.</param>
        public ClientInfoPacket(bool isMobile, UserAgent userAgent)
        {
            Header = "_cinfo";

            Data = new JObject();
            Data["mobile"] = isMobile;
            Data["cver"] = userAgent.ToString();

            Data["adf"] = "php"; // FIXME: Currently hard-coded, new protocol analysis yielded just this result.
        }
    }
}
