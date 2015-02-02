using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class PongPacket : Packet
    {
        public PongPacket()
        {
            Header = "_gdzie";
        }
    }
}
