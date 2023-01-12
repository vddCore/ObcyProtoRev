using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class ChatstatePacket : Packet
    {
        public ChatstatePacket(bool typing, string strangerUid)
        {
            Header = "_mtyp";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
                ["val"] = typing
            };
        }
    }
}
