using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class PongPacket : Packet
    {
        public PongPacket()
        {
            Header = "_gdzie";
        }
    }
}
