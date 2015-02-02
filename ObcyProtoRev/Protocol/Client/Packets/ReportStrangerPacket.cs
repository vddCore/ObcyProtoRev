using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class ReportStrangerPacket : Packet
    {
        public ReportStrangerPacket(string clientGuid)
        {
            Header = "_reptalk";

            Data = new JObject();
            Data["ckey"] = clientGuid;
        }
    }
}
