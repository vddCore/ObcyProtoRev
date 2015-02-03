using System;
using ObcyProtoRev.Protocol.Client;
using ObcyProtoRev.Protocol.SockJs;
using WebSocketSharp;

namespace ObcyProtoRev.Protocol
{
    public class Connection
    {
        private WebSocket WebSocket { get; set; }
        private TargetWebsocketAddress WebsocketAddress { get; set; }

        public bool IsReady { get; private set; }
        public bool IsOpen { get; private set; }

        private PacketHandler IncomingPacketHandler { get; set; }

        public Connection()
        {
            RenewConnectionAddress();
            CreateWebsocket();

            IncomingPacketHandler = new PacketHandler(this);

            IsReady = true;
        }

        public void RenewConnectionAddress()
        {
            WebSocket.Close();
            IsReady = false;

            WebsocketAddress = new TargetWebsocketAddress();
            IsReady = true;
        }

        public void SendPacket(Packet packet)
        {
            WebSocket.Send(packet);
        }

        private void CreateWebsocket()
        {
            WebSocket = new WebSocket(WebsocketAddress)
            {
                Origin = WebsocketAddress.Origin
            };

            WebSocket.OnOpen += WebSocket_OnOpen;
            WebSocket.OnMessage += WebSocket_OnMessage;
            WebSocket.OnError += WebSocket_OnError;
            WebSocket.OnClose += WebSocket_OnClose;
        }

        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            IsOpen = true;
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            var packets = Decoder.DecodePackets(messageEventArgs.Data);
            Packet packet = packets.GetFirstPacket();

            PacketType packetType = Decoder.DeterminePacketType(packet);

            switch (packetType)
            {
                case PacketType.ConnectionOpen:
                    break;
                case PacketType.ConnectionClose:
                    break;
                case PacketType.SocketHeartbeat:
                    break;
                case PacketType.SocketMessage:
                    break;
                case PacketType.BinaryData:
                    break;
                case PacketType.Invalid:
                    break;
            }
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("connection error, prolly closed");
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            IsOpen = false;
        }

    }
}
