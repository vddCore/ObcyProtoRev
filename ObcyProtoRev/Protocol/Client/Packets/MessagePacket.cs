using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class MessagePacket : Packet
    {
        public MessagePacket(string message, string contactGuid)
        {
            Header = "_pmsg";

            Data = new JObject();
            Data["ckey"] = contactGuid;
            Data["msg"] = message;
        } 
    }
}
