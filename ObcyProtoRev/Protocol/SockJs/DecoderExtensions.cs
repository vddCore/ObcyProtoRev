using System.Collections.Generic;
using System.Linq;

namespace ObcyProtoRev.Protocol.SockJs
{
    static class DecoderExtensions
    {
        public static Packet GetFirstPacket(this List<Packet> packets)
        {
            return packets.FirstOrDefault();
        }
    }
}
