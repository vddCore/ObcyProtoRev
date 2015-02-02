using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class RandomTopicPacket : Packet
    {
        public RandomTopicPacket(string contactGuid)
        {
            Header = "_randtopic";

            Data = new JObject();
            Data["ckey"] = contactGuid;
        }
    }
}
