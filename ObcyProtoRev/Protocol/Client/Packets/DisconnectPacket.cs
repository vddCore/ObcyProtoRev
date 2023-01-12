using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class DisconnectPacket : Packet
    {
        public DisconnectPacket(string contactGuid)
        {
            Header = "_distalk";

            Data = new JObject();
            Data["ckey"] = contactGuid;
        }
    }
}
