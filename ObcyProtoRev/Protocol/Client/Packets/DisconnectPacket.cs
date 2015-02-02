using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class DisconnectPacket : Packet
    {
        public DisconnectPacket(string contactGuid)
        {
            Header = "_distalk";

            var jObject = new JObject();
            jObject["ckey"] = contactGuid;
        }
    }
}
