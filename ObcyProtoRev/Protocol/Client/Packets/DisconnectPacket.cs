using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class DisconnectPacket : Packet
    {
        public DisconnectPacket(string strangerUID)
        {
            Header = "_distalk";

            Data = new JObject
            {
                ["ckey"] = strangerUID,
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}
