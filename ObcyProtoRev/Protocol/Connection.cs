using System;
using System.Collections.Generic;

using ObcyProtoRev.Protocol.Client;
using ObcyProtoRev.Protocol.Client.Packets;
using ObcyProtoRev.Protocol.Server.Packets;

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

        public delegate void ConnectionAcceptedEventHandler(object sender, string connectionId);
        public delegate void ConversationEndedEventHandler(object sender, string clientId);
        public delegate void MessageEventHandler(object sender, Message message);        
        public delegate void OnlinePeopleCountChangedEventHandler(object sender, int count);
        public delegate void StrangerChatstateChangedEventHandler(object sender, bool typing);
        public delegate void StrangerFoundEventHandler(object sender, ContactInfo contactInfo);

        public event ConnectionAcceptedEventHandler ConnectionAccepted;
        public event ConversationEndedEventHandler ConversationEnded;
        public event MessageEventHandler MessageReceived;
        public event OnlinePeopleCountChangedEventHandler OnlinePeopleCountChanged;
        public event StrangerChatstateChangedEventHandler StrangerChatstateChanged;
        public event StrangerFoundEventHandler StrangerFound;

        public Connection()
        {
            CreateWebsocket();
            RegisterPacketHandlerEvents();
            RenewConnectionAddress();

            IsReady = true;
        }

        public void SendPacket(Packet packet)
        {
            if(IsReady && IsOpen)
                WebSocket.Send(packet);
        }

        public void SendPacket(string json)
        {
            if (IsReady && IsOpen)
                WebSocket.Send(json);
        }

        #region Private methods
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

        private void RegisterPacketHandlerEvents()
        {
            WebsocketPacketHandler.ConnectionOpenPacketReceived += WebsocketPacketHandler_ConnectionOpenPacketReceived;
            WebsocketPacketHandler.ConnectionClosePacketReceived += WebsocketPacketHandlerOnConnectionClosePacketReceived;
            WebsocketPacketHandler.SocketHeartbeatReceived += WebsocketPacketHandlerOnSocketHeartbeatReceived;
            WebsocketPacketHandler.SocketMessageReceived += WebsocketPacketHandler_SocketMessageReceived;
        }

        private void RenewConnectionAddress()
        {
            if (WebSocket.IsAlive)
            {
                WebSocket.Close();
                IsReady = false;
            }

            WebsocketAddress = new TargetWebsocketAddress();
            IsReady = true;
        }
        #endregion

        #region Packet handler events
        private void WebsocketPacketHandler_SocketMessageReceived(List<Packet> packets)
        {
            foreach (var packet in packets)
            {
                if (packet.Header == ConnectionAcceptedPacket.ToString())
                {
                    OnConnectionAccepted(packet.Data["conn_id"].ToString());
                }

                if (packet.Header == ConversationEndedPacket.ToString())
                {
                    OnConversationEnded(
                        packet.Data.ToString()
                    );
                }

                if (packet.Header == MessageReceivedPacket.ToString())
                {
                    var message = new Message(
                        MessageType.Instant,
                        packet.Data["msg"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.Data["post_id"].ToString())
                    );
                    OnMessageReceived(message);
                }

                if (packet.Header == OnlinePeopleCountPacket.ToString())
                {
                    OnOnlinePeopleCountChanged(
                        int.Parse(packet.Data.ToString())
                    );
                }

                if (packet.Header == PingPacket.ToString())
                {
                    SendPacket(new PongPacket());
                }

                if (packet.Header == RandomTopicReceivedPacket.ToString())
                {
                    var message = new Message(
                        MessageType.Subject,
                        packet.Data["topic"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.Data["post_id"].ToString())
                    );
                    OnMessageReceived(message);
                }

                if (packet.Header == ReconnectionSuccessPacket.ToString())
                {
                    
                }

                if (packet.Header == ServiceMessageReceivedPacket.ToString())
                {
                    var message = new Message(MessageType.Service, packet.Data.ToString(), null, null);
                    OnMessageReceived(message);                    
                }

                if (packet.Header == StrangerChatstatePacket.ToString())
                {
                    OnStrangerChatstateChanged(
                        bool.Parse(packet.Data.ToString())
                    );
                }

                if (packet.Header == StrangerDisconnectedPacket.ToString())
                {
                    
                }

                if (packet.Header == StrangerFoundPacket.ToString())
                {
                    
                }
            }
        }

        private void WebsocketPacketHandlerOnSocketHeartbeatReceived(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private void WebsocketPacketHandlerOnConnectionClosePacketReceived(string sockJsPacket)
        {
            throw new NotImplementedException();
        }

        private void WebsocketPacketHandler_ConnectionOpenPacketReceived(string sockJsPacket)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Low-level websocket events
        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            IsOpen = true;
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            WebsocketPacketHandler.HandlePacket(messageEventArgs.Data);
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            IsOpen = false;
        }
        #endregion

        protected virtual void OnConnectionAccepted(string connectionid)
        {
            var handler = ConnectionAccepted;
            if (handler != null) handler(this, connectionid);
        }

        protected virtual void OnMessageReceived(Message message)
        {
            var handler = MessageReceived;
            if (handler != null) handler(this, message);
        }

        protected virtual void OnOnlinePeopleCountChanged(int count)
        {
            var handler = OnlinePeopleCountChanged;
            if (handler != null) handler(this, count);
        }

        protected virtual void OnStrangerFound(ContactInfo contactinfo)
        {
            var handler = StrangerFound;
            if (handler != null) handler(this, contactinfo);
        }

        protected virtual void OnConversationEnded(string clientid)
        {
            var handler = ConversationEnded;
            if (handler != null) handler(this, clientid);
        }

        protected virtual void OnStrangerChatstateChanged(bool typing)
        {
            var handler = StrangerChatstateChanged;
            if (handler != null) handler(this, typing);
        }
    }
}
