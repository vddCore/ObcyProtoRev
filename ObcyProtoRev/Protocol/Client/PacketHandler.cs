using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client
{
    class PacketHandler
    {
        private Connection UnderlyingConnection { get; set; }

        public PacketHandler(Connection underlyingConnection)
        {
            UnderlyingConnection = underlyingConnection;
        }

        public void HandlePacket(Packet packet)
        {
            
        }
    }
}
