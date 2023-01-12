using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class ClientInfoPacket : Packet
    {
        public ClientInfoPacket(bool isMobile, UserAgent userAgent)
        {
            Header = "_cinfo";

            Data = new JObject
            {
                ["mobile"] = isMobile,
                ["cver"] = userAgent.ToString(), 
                ["adf"] = "ajaxPHP" // FIXME: Currently hard-coded, new protocol analysis yielded just this result.
            };

        }
    }
}
