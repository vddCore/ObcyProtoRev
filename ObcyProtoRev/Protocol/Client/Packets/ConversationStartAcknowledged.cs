using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public class ConversationStartAcknowledged : Packet
    {
        public ConversationStartAcknowledged(string strangerUID)
        {
            Header = "_begacked";

            Data = new JObject();
            Data["ckey"] = strangerUID;

            Data["ceid"] = Connection.ActionID;
        }
    }
}
