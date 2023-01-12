using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public class ConversationStartAcknowledged : Packet
    {
        public ConversationStartAcknowledged(string strangerUid)
        {
            Header = "_begacked";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}
