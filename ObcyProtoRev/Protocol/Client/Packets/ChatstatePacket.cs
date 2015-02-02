using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class ChatstatePacket : Packet
    {
        public ChatstatePacket(bool typing, string contactGuid)
        {
            Header = "_mtyp";

            Data = new JObject();
            Data["ckey"] = contactGuid;
            Data["val"] = typing;
        }
    }
}
