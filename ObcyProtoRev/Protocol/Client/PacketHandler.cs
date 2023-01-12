using System;
using System.Collections.Generic;
using System.Diagnostics;

using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client
{
    static class WebsocketPacketHandler
    {
        public delegate void ConnectionOpenEventHandler(EventArgs e);
        public delegate void ConnectionCloseEventHandler(EventArgs e);
        public delegate void SocketHeartbeatEventHandler(DateTime heartbeatTime);
        public delegate void SocketMessageEventHandler(List<Packet> packets);

        public static event ConnectionOpenEventHandler ConnectionOpenPacketReceived;
        public static event ConnectionCloseEventHandler ConnectionClosePacketReceived;
        public static event SocketHeartbeatEventHandler SocketHeartbeatReceived;
        public static event SocketMessageEventHandler SocketMessageReceived;

        public static void HandlePacket(string sockJsPacket)
        {
            var packetType = Decoder.DeterminePacketType(sockJsPacket);

            switch (packetType)
            {
                case PacketType.ConnectionOpen:
                    ConnectionOpenPacketReceived?.Invoke(EventArgs.Empty);
                    break;
                case PacketType.ConnectionClose:
                    ConnectionClosePacketReceived?.Invoke(EventArgs.Empty);
                    break;
                case PacketType.SocketHeartbeat:
                    SocketHeartbeatReceived?.Invoke(DateTime.Now);
                    break;
                case PacketType.SocketMessage:
                    var packets = Decoder.DecodePackets(sockJsPacket);
                    SocketMessageReceived?.Invoke(packets);
                    break;
                case PacketType.BinaryData:
                    Debug.WriteLine("Binary data not supported.");
                    break;
                default: // Includes PacketType.Invalid
                    Debug.WriteLine("Invalid packet received.");
                    break;
            }
        }
    }
}
