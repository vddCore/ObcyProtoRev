using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class RandomTopicPacket : Packet
    {
        public RandomTopicPacket(string strangerUID)
        {
            Header = "_randtopic";

            Data = new JObject
            {
                ["ckey"] = strangerUID
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}
