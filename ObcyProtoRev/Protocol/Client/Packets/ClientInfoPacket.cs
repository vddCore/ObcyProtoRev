using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class ClientInfoPacket : Packet
    {
        public ClientInfoPacket(bool isMobile, UserAgent userAgent)
        {
            Header = "_cinfo";

            var jObject = new JObject();
            jObject["mobile"] = isMobile;
            jObject["cver"] = userAgent.ToString();

            Data = jObject;
        }
    }
}
