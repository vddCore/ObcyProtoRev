using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class ReconnectRequestPacket : Packet
    {
        public ReconnectRequestPacket(ReconnectInfo reconnectInfo)
        {
            Header = "_reconn_me";

            //TODO: Stub
        }
    }
}
