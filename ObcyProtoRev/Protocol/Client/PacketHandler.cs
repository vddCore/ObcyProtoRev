using System;
using System.Collections.Generic;
using System.Diagnostics;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client
{
    static class WebsocketPacketHandler
    {
        public delegate void ConnectionOpenEventHandler(string sockJsPacket);
        public delegate void ConnectionCloseEventHandler(string sockJsPacket);
        public delegate void SocketHeartbeatEventHandler(EventArgs e);
        public delegate void SocketMessageEventHandler(List<Packet> packets);

        public static event ConnectionOpenEventHandler ConnectionOpenPacketReceived;
        public static event ConnectionCloseEventHandler ConnectionClosePacketReceived;
        public static event SocketHeartbeatEventHandler SocketHeartbeatReceived;
        public static event SocketMessageEventHandler SocketMessageReceived;

        public static void HandlePacket(string sockJsPacket)
        {
            PacketType packetType = Decoder.DeterminePacketType(sockJsPacket);

            switch (packetType)
            {
                case PacketType.ConnectionOpen:
                    OnConnectionOpenPacketReceived(sockJsPacket);
                    break;
                case PacketType.ConnectionClose:
                    OnConnectionClosePacketReceived(sockJsPacket);
                    break;
                case PacketType.SocketHeartbeat:
                    OnSocketHeartbeatReceived();
                    break;
                case PacketType.SocketMessage:
                    var packets = Decoder.DecodePackets(sockJsPacket);
                    OnSocketMessageReceived(packets);
                    break;
                case PacketType.BinaryData:
                    Debug.WriteLine("Binary data not supported.");
                    break;
                case PacketType.Invalid:
                    Debug.WriteLine("Invalid packet received.");
                    break;
            }
        }

        private static void OnConnectionOpenPacketReceived(string sockJsPacket)
        {
            var handler = ConnectionOpenPacketReceived;
            if (handler != null) handler(sockJsPacket);
        }

        private static void OnConnectionClosePacketReceived(string sockJsPacket)
        {
            var handler = ConnectionClosePacketReceived;
            if (handler != null) handler(sockJsPacket);
        }

        private static void OnSocketHeartbeatReceived()
        {
            var handler = SocketHeartbeatReceived;
            if (handler != null) handler(EventArgs.Empty);
        }

        private static void OnSocketMessageReceived(List<Packet> packets)
        {
            var handler = SocketMessageReceived;
            if (handler != null) handler(packets);
        }
    }
}
