using System;
using System.Collections.Generic;

using ObcyProtoRev.Protocol.Client;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.Client.Packets;
using ObcyProtoRev.Protocol.Server.Packets;
using ObcyProtoRev.Protocol.SockJs;

using WebSocketSharp;

namespace ObcyProtoRev.Protocol
{
    public class Connection
    {
        #region Private fields
        private WebSocket WebSocket { get; set; }
        private TargetWebsocketAddress WebsocketAddress { get; set; }
        #endregion

        #region Public fields
        public bool IsReady { get; private set; }
        public bool IsOpen { get; private set; }
        public bool IsMobile { get; set; }
        public bool IsStrangerConnected { get; private set; }
        public bool KeepAlive { get; set; }
        public bool SendUserAgent { get; set; }

        public string CurrentContactUID { get; private set; }
        public UserAgent UserAgent { get; set; }
        #endregion

        #region Delegates
        public delegate void BooleanEventHandler(object sender, bool value);
        public delegate void ContactInfoEventHandler(object sender, ContactInfo contactInfo);
        public delegate void ConversationEndEventHandler(object sender, DisconnectInfo disconnectInfo);
        public delegate void DateTimeEventHandler(object sender, DateTime dateTime);
        public delegate void ErrorEventHandler(object sender, Exception e);
        public delegate void IntegerEventHandler(object sender, int count);
        public delegate void MessageEventHandler(object sender, Message message);
        public delegate void ObjectEventHandler(object sender, EventArgs e);
        public delegate void StringEventHandler(object sender, string value);
        #endregion

        #region Event handlers
        public event StringEventHandler ConnectionAccepted;
        public event ObjectEventHandler ConnectionAcknowledeged;
        public event ConversationEndEventHandler ConversationEnded;
        public event DateTimeEventHandler HeartbeatReceived;
        public event StringEventHandler JsonRead;
        public event StringEventHandler JsonWritten;
        public event MessageEventHandler MessageReceived;
        public event IntegerEventHandler OnlinePeopleCountChanged;
        public event DateTimeEventHandler PingReceived;
        public event ObjectEventHandler ServerClosedConnection;
        public event StringEventHandler SocketClosed;
        public event ErrorEventHandler SocketError;
        public event ObjectEventHandler SocketOpened;
        public event BooleanEventHandler StrangerChatstateChanged;
        public event ContactInfoEventHandler StrangerFound;
        #endregion

        #region Constructor
        public Connection()
        {
            RenewConnectionAddress();
            CreateWebsocket();
            RegisterPacketHandlerEvents();

            KeepAlive = true;
            IsReady = true;

            SendUserAgent = true;
            UserAgent = new UserAgent("ObcyProtoRev", "0.3.0.0");
        }
        #endregion

        #region Public methods
        public void Close()
        {
            if (IsReady && IsOpen)
                WebSocket.Close();
        }

        public void Open()
        {
            if (IsReady && !IsOpen)
                WebSocket.Connect();
        }

        public void DisconnectStranger()
        {
            if (IsStrangerConnected)
            {
                SendPacket(
                    new DisconnectPacket(CurrentContactUID)
                );
            }
        }

        public void FlagStranger()
        {
            if (IsOpen && IsReady)
            {
                SendPacket(
                    new ReportStrangerPacket(CurrentContactUID)
                );
            }
        }

        public void PongResponse()
        {
            if (IsOpen && IsReady)
            {
                SendPacket(
                    new PongPacket()
                );
            }
        }

        public void ReportChatstate(bool typing)
        {
            if (IsStrangerConnected)
            {
                SendPacket(
                    new ChatstatePacket(typing, CurrentContactUID)
                );
            }
        }

        public void RequestRandomTopic()
        {
            if (IsStrangerConnected)
            {
                SendPacket(
                    new RandomTopicPacket(CurrentContactUID)
                );
            }
        }

        public void SearchForStranger(Location location)
        {
            if (!IsStrangerConnected)
            {
                var info = new PersonInfo(0, location);

                SendPacket(
                    new StrangerSearchPacket(info, info, "main")
                );
            }
        }

        public void SendMessage(string message)
        {
            if (IsReady && IsOpen)
            {
                SendPacket(
                    new MessagePacket(message, CurrentContactUID)
                );
            }
        }

        public void SendPacket(Packet packet)
        {
            if (IsReady && IsOpen)
                WebSocket.Send(packet);

            OnJsonWrite(packet);
        }

        public void SendPacket(string json)
        {
            if (IsReady && IsOpen)
                WebSocket.Send(json);

            OnJsonWrite(json);
        }
        #endregion

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
            if (WebSocket != null)
            {
                if (WebSocket.IsAlive)
                {
                    WebSocket.Close();
                    IsReady = false;
                }
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
                        new DisconnectInfo(
                            true,
                            int.Parse(packet.Data.ToString())
                        )
                    );
                }

                if (packet.Header == MessageReceivedPacket.ToString())
                {                   
                    var message = new Message(
                        MessageType.Instant,
                        packet.Data["msg"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.AdditionalFields["post_id"].ToString())
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
                    if (KeepAlive)
                        PongResponse();

                    OnPingReceived(DateTime.Now);
                }

                if (packet.Header == RandomTopicReceivedPacket.ToString())
                {
                    var message = new Message(
                        MessageType.Subject,
                        packet.Data["topic"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.AdditionalFields["post_id"].ToString())
                    );
                    OnMessageReceived(message);
                }

                if (packet.Header == ReconnectionSuccessPacket.ToString())
                {
                    // TODO: Requires more reverse-engineering.
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
                    OnConversationEnded(new DisconnectInfo(
                            false,
                            int.Parse(packet.Data.ToString())
                        )
                    );
                }

                if (packet.Header == StrangerFoundPacket.ToString())
                {
                    CurrentContactUID = packet.Data["ckey"].ToString();

                    OnStrangerFound(
                        new ContactInfo(
                            int.Parse(packet.Data["cid"].ToString()),
                            packet.Data["ckey"].ToString(),
                            packet.Data["info"],
                            bool.Parse(packet.Data["flaged"].ToString())
                        )
                    );
                }
            }
        }

        private void WebsocketPacketHandlerOnSocketHeartbeatReceived(DateTime heartbeatTime)
        {
            OnHeartbeatReceived(heartbeatTime);
        }

        private void WebsocketPacketHandlerOnConnectionClosePacketReceived(EventArgs e)
        {
            OnServerClosedConnection();
        }

        private void WebsocketPacketHandler_ConnectionOpenPacketReceived(EventArgs e)
        {
            OnConnectionAcknowledeged();
        }
        #endregion

        #region Low-level websocket events
        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            OnSocketOpened();
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            WebsocketPacketHandler.HandlePacket(messageEventArgs.Data);
            OnJsonRead(messageEventArgs.Data);
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            OnSocketError(e.Exception);
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            OnSocketClosed(e.Reason);
        }
        #endregion

        #region Event invokers
        protected virtual void OnConnectionAccepted(string connectionid)
        {
            var handler = ConnectionAccepted;
            
            if (handler != null) 
                handler(this, connectionid);

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

            if (handler != null)
                handler(this, contactinfo);

            IsStrangerConnected = true;
        }

        protected virtual void OnConversationEnded(DisconnectInfo disconnectInfo)
        {
            var handler = ConversationEnded;

            if (handler != null)
                handler(this, disconnectInfo);

            IsStrangerConnected = false;
        }

        protected virtual void OnStrangerChatstateChanged(bool typing)
        {
            var handler = StrangerChatstateChanged;
            if (handler != null) handler(this, typing);
        }

        protected virtual void OnPingReceived(DateTime pingtime)
        {
            var handler = PingReceived;
            if (handler != null) handler(this, pingtime);
        }

        protected virtual void OnConnectionAcknowledeged()
        {
            var handler = ConnectionAcknowledeged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnJsonRead(string value)
        {
            var handler = JsonRead;
            if (handler != null) handler(this, value);
        }

        protected virtual void OnJsonWrite(string value)
        {
            var handler = JsonWritten;
            if (handler != null) handler(this, value);
        }

        protected virtual void OnServerClosedConnection()
        {
            var handler = ServerClosedConnection;

            if (handler != null)
                handler(this, EventArgs.Empty);

            IsOpen = false;
        }

        protected virtual void OnHeartbeatReceived(DateTime datetime)
        {
            var handler = HeartbeatReceived;
            if (handler != null) handler(this, datetime);
        }

        protected virtual void OnSocketClosed(string reason)
        {
            var handler = SocketClosed;

            if (handler != null)
                handler(this, reason);

            IsOpen = false;
        }

        protected virtual void OnSocketError(Exception e)
        {
            var handler = SocketError;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnSocketOpened()
        {
            var handler = SocketOpened;

            if (handler != null)
                handler(this, EventArgs.Empty);

            IsOpen = true;
        }
        #endregion
    }
}
