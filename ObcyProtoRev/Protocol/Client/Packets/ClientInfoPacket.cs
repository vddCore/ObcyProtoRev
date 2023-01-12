using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class ClientInfoPacket : Packet
    {
        public ClientInfoPacket(bool isMobile, UserAgent userAgent)
        {
            Header = "_cinfo";

            Data = new JObject();
            Data["mobile"] = isMobile;
            Data["cver"] = userAgent.ToString();
        }
    }
}
