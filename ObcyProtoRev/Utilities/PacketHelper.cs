using ObcyProtoRev.Protocol.Server.Packets;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Utilities
{
    public static class PacketHelper
    {
        public static bool IsConnectionAcceptedPacket(Packet packet)
        {
            return packet.Header == ConnectionAcceptedPacket.ToString();
        }

        public static bool IsConversationEndedPacket(Packet packet)
        {
            return packet.Header == ConversationEndedPacket.ToString();
        }

        public static bool IsMessageReceivedPacket(Packet packet)
        {
            return packet.Header == MessageReceivedPacket.ToString();
        }

        public static bool IsOnlinePeopleCountPacket(Packet packet)
        {
            return packet.Header == OnlinePeopleCountPacket.ToString();
        }

        public static bool IsPingPacket(Packet packet)
        {
            return packet.Header == PingPacket.ToString();
        }

        public static bool IsRandomTopicReceivedPacket(Packet packet)
        {
            return packet.Header == RandomTopicReceivedPacket.ToString();
        }

        public static bool IsReconnectionSuccessPacket(Packet packet)
        {
            return packet.Header == ReconnectionSuccessPacket.ToString();
        }

        public static bool IsServiceMessageReceivedPacket(Packet packet)
        {
            return packet.Header == ServiceMessageReceivedPacket.ToString();
        }

        public static bool IsStrangerChatstatePacket(Packet packet)
        {
            return packet.Header == StrangerChatstatePacket.ToString();
        }

        public static bool IsStrangerDisconnectedPacket(Packet packet)
        {
            return packet.Header == StrangerDisconnectedPacket.ToString();
        }

        public static bool IsStrangerFoundPacket(Packet packet)
        {
            return packet.Header == StrangerFoundPacket.ToString();
        }
    }
}
