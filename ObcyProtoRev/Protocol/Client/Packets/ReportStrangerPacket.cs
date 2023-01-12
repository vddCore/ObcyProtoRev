using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a report-as-unpleasant packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class ReportStrangerPacket : Packet
    {
        /// <summary>
        /// Creates a new instance of ReportStrangerPacket class.
        /// </summary>
        /// <param name="strangerUID">UID of a stranger which we want to report.</param>
        public ReportStrangerPacket(string strangerUID)
        {
            Header = "_reptalk";

            Data = new JObject();
            Data["ckey"] = strangerUID;

            Data["ceid"] = Connection.ActionID;
        }
    }
}
