using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class ReportStrangerPacket : Packet
    {
        public ReportStrangerPacket(string strangerUid)
        {
            Header = "_reptalk";

            Data = new JObject
            {
                ["ckey"] = strangerUid
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}
