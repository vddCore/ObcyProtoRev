using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class MessagePacket : Packet
    {
        public MessagePacket(string body, string strangerUid)
        {
            Header = "_pmsg";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
                ["msg"] = body,
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}
