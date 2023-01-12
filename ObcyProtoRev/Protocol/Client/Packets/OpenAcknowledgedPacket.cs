using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class OpenAcknowledgedPacket : Packet
    {
        public OpenAcknowledgedPacket()
        {
            Header = "_owack";
        }
    }
}
