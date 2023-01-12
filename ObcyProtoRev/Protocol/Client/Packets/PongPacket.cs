using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a pong packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class PongPacket : Packet
    {
        /// <summary>
        /// Creates a new instance of PongPacket class.
        /// </summary>
        public PongPacket()
        {
            Header = "_gdzie";
        }
    }
}
