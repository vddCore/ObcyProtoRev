using System;
using System.Collections.Generic;
using System.Diagnostics;
using ObcyProtoRev.Protocol.Client;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.Client.Packets;
using ObcyProtoRev.Protocol.Server.Packets;
using ObcyProtoRev.Protocol.SockJs;

using WebSocketSharp;

namespace ObcyProtoRev.Protocol
{
    /// <summary>
    /// The main service connection class.
    /// </summary>
    public class Connection
    {
        #region Private fields
        /// <summary>
        /// Gets or sets connection's underlying websocket.
        /// </summary>
        private WebSocket WebSocket { get; set; }

        /// <summary>
        /// Gets or sets current generated target websocket address.
        /// </summary>
        private TargetWebsocketAddress WebsocketAddress { get; set; }
        #endregion

        #region Public fields
        /// <summary>
        /// Gets a value indicating that socket is ready to open a connection.
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Gets a value indicating that connection is already open.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Gets or sets whether a connection should report itself as mobile.
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        /// Gets a value indicating that a stranger is connected.
        /// </summary>
        public bool IsStrangerConnected { get; private set; }

        /// <summary>
        /// Gets a value indicating that a search for a stranger is currently ongoing.
        /// </summary>
        public bool IsSearchingForStranger { get; private set; }

        /// <summary>
        /// Gets or sets whether the connection should be kept alive automatically.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets whether the connection should send its identity.
        /// </summary>
        public bool SendUserAgent { get; set; }

        /// <summary>
        /// Gets a value indicating current stranger UID assigned by the service.
        /// </summary>
        public string CurrentContactUID { get; private set; }

        /// <summary>
        /// Gets or sets an identity that should be sent on connection acknowledge.
        /// </summary>
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
        /// <summary>
        /// Event that gets fired after the client receives "ConnectionAccepted" service packet.
        /// </summary>
        public event StringEventHandler ConnectionAccepted;

        /// <summary>
        /// Event that gets fired after the client receives the meaningful "o" socket packet.
        /// </summary>
        public event ObjectEventHandler ConnectionAcknowledged;

        /// <summary>
        /// Event that gets fired when a stranger disconnects or client tries to send a packet that requires an active conversation.
        /// </summary>
        public event ConversationEndEventHandler ConversationEnded;

        /// <summary>
        /// Event that gets fired when the client receives the meaningful "h" socket packet.
        /// </summary>
        public event DateTimeEventHandler HeartbeatReceived;

        /// <summary>
        /// Event that gets fired when any (either service or socket) packet has been received.
        /// </summary>
        public event StringEventHandler JsonRead;

        /// <summary>
        /// Event that gets fired when any (either service or socket) packet has been sent.
        /// </summary>
        public event StringEventHandler JsonWritten;

        /// <summary>
        /// Event that gets fired when the client receives a message from the currently connected stranger.
        /// </summary>
        public event MessageEventHandler MessageReceived;

        /// <summary>
        /// Event that gets fired when client receives "OnlineCount" service packet.
        /// </summary>
        public event IntegerEventHandler OnlinePeopleCountChanged;

        /// <summary>
        /// Event that gets fired when client receives "Ping" service packet.
        /// </summary>
        public event DateTimeEventHandler PingReceived;

        /// <summary>
        /// Event that gets fired when client receives the meaningful "c" socket packet.
        /// </summary>
        public event ObjectEventHandler ServerClosedConnection;

        /// <summary>
        /// Event that gets fired when socket gets closed.
        /// </summary>
        public event StringEventHandler SocketClosed;

        /// <summary>
        /// Event that gets fired when socket encounters an error.
        /// </summary>
        public event ErrorEventHandler SocketError;

        /// <summary>
        /// Event that gets fired when socket gets opened.
        /// </summary>
        public event ObjectEventHandler SocketOpened;

        /// <summary>
        /// Event that gets fired when client receives Chatstate packet, i.e. stranger is or is not typing.
        /// </summary>
        public event BooleanEventHandler StrangerChatstateChanged;

        /// <summary>
        /// Event that gets fired when client receives "StrangerFound" packet and the conversation has started.
        /// </summary>
        public event ContactInfoEventHandler StrangerFound;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Connection instance.
        /// </summary>
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
        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            if (IsReady && IsOpen)
                WebSocket.Close();
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void Open()
        {
            if (IsReady && !IsOpen)
                WebSocket.Connect();
        }

        /// <summary>
        /// Disconnects from currently connected stranger.
        /// </summary>
        public void DisconnectStranger()
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                SendPacket(
                    new DisconnectPacket(CurrentContactUID)
                );
                IsStrangerConnected = false;

                DisconnectInfo di = new DisconnectInfo(false, 0);
                OnConversationEnded(di);
            }
        }

        /// <summary>
        /// Reports currently or last connected stranger as malicious.
        /// </summary>
        public void FlagStranger()
        {
            if (IsOpen && IsReady)
            {
                SendPacket(
                    new ReportStrangerPacket(CurrentContactUID)
                );
            }
        }

        /// <summary>
        /// Sends a "pong" packet to the server.
        /// </summary>
        public void PongResponse()
        {
            if (IsOpen && IsReady)
            {
                SendPacket(
                    new PongPacket()
                );
            }
        }

        /// <summary>
        /// Sends a chatstate, i.e. notification whether or not the client is typing on keyboard.
        /// </summary>
        /// <param name="isTyping">Set to <see langword="true"/> if typing, set to <see langword="false"/> if not.</param>
        public void ReportChatstate(bool isTyping)
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                SendPacket(
                    new ChatstatePacket(isTyping, CurrentContactUID)
                );
            }
        }

        /// <summary>
        /// Sends a request to the service for a random topic shuffle.
        /// </summary>
        public void RequestRandomTopic()
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                SendPacket(
                    new RandomTopicPacket(CurrentContactUID)
                );
            }
        }

        /// <summary>
        /// Searches for a stranger in requested location.
        /// </summary>
        /// <param name="targetLocation">Desired location to search in.</param>
        public void SearchForStranger(Location targetLocation)
        {
            if (IsReady && IsOpen && !IsStrangerConnected)
            {
                if (!IsSearchingForStranger)
                {
                    IsSearchingForStranger = true;

                    var info = new PersonInfo(0, targetLocation);

                    SendPacket(
                        new StrangerSearchPacket(info, info, "main")
                    );
                }
            }
        }

        /// <summary>
        /// Sends a message to the currently connected stranger.
        /// </summary>
        /// <param name="message">Message content to be sent.</param>
        public void SendMessage(string message)
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                SendPacket(
                    new MessagePacket(message, CurrentContactUID)
                );
            }
        }

        /// <summary>
        /// Sends a predefined, tested and safe packet.
        /// </summary>
        /// <param name="packet">Packet to be sent.</param>
        public void SendPacket(Packet packet)
        {
            if (IsReady && IsOpen)
                WebSocket.Send(packet);

            OnJsonWrite(packet);
        }

        /// <summary>
        /// Sends a free-form, unsafe packet in JSON format.
        /// </summary>
        /// <param name="json">JSON packet string to be sent.</param>
        /// <remarks>Make sure the packet is WELL formed. Otherwise client will certainly get disconnected.</remarks>
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
            // Server may send more than one packet.
            // -------------------------------------
            foreach (var packet in packets)
            {
                // Why so many "if" statements out there?
                // I guess it's because switch becomes spaghetti-code at some point.
                // You know, I'm not hungry, and you've gotta admit it:
                // these conditionals are pretty good-looking.
                // ---------------------------------------------------------------

                if (packet.Header == ConnectionAcceptedPacket.ToString())
                {
                    Debug.Assert(packet.Data != null, "ConnectionAccepted: packet.Data != null");

                    OnConnectionAccepted(packet.Data["conn_id"].ToString());
                }

                if (packet.Header == ConversationEndedPacket.ToString())
                {
                    // Unusual behavior, server sends "convended" without any data
                    // if "flag stranger" packet is sent and no conversation have
                    // been started before.
                    //
                    // Hence, we have to handle it like this.
                    // -----------------------------------------------------------
                    if (packet.Data != null)
                    {
                        OnConversationEnded(
                            new DisconnectInfo(
                                true,
                                int.Parse(packet.Data.ToString())
                            )
                        );
                    }
                    else
                    {
                        OnConversationEnded(
                            new DisconnectInfo(
                                true,
                                0
                            )
                        );
                    }
                }

                if (packet.Header == StrangerDisconnectedPacket.ToString())
                {
                    Debug.Assert(packet.Data != null, "StrangerDisconnected: packet.Data != null");

                    OnConversationEnded(
                        new DisconnectInfo(
                            false,
                            int.Parse(packet.Data.ToString())
                        )
                    );
                }

                if (packet.Header == MessageReceivedPacket.ToString())
                {
                    Debug.Assert(packet.Data != null, "MessageReceived: packet.Data != null");

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
                    Debug.Assert(packet.Data != null, "OnlineCountChanged: packet.Data != null");

                    OnOnlinePeopleCountChanged(
                        int.Parse(packet.Data.ToString())
                    );
                }

                if (packet.Header == PingPacket.ToString())
                {
                    // No assertion or special handling needed.
                    // This packet does not have any data.
                    // ----------------------------------------
                    if (KeepAlive)
                        PongResponse();

                    OnPingReceived(DateTime.Now);
                }

                if (packet.Header == RandomTopicReceivedPacket.ToString())
                {
                    Debug.Assert(packet.Data != null, "RandomTopicReceived: packet.Data != null");

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
                    Debug.Assert(packet.Data != null, "ServiceMessageReceived: packet.Data != null");

                    var message = new Message(MessageType.Service, packet.Data.ToString(), null, null);
                    OnMessageReceived(message);
                }

                if (packet.Header == StrangerChatstatePacket.ToString())
                {
                    Debug.Assert(packet.Data != null, "StrangerChatstateChanged: packet.Data != null");

                    OnStrangerChatstateChanged(
                        bool.Parse(packet.Data.ToString())
                    );
                }

                if (packet.Header == StrangerFoundPacket.ToString())
                {
                    Debug.Assert(packet.Data != null, "StrangerFound: packet.Data != null");

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
            OnConnectionAcknowledged();
        }
        #endregion

        #region Low-level websocket events
        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            OnSocketOpened();
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            OnJsonRead(messageEventArgs.Data);
            WebsocketPacketHandler.HandlePacket(messageEventArgs.Data);
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
            IsSearchingForStranger = false;
            IsStrangerConnected = true;

            var handler = StrangerFound;

            if (handler != null)
                handler(this, contactinfo);
        }

        protected virtual void OnConversationEnded(DisconnectInfo disconnectInfo)
        {
            IsStrangerConnected = false;

            var handler = ConversationEnded;

            if (handler != null)
                handler(this, disconnectInfo);
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

        protected virtual void OnConnectionAcknowledged()
        {
            var handler = ConnectionAcknowledged;
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
            IsOpen = false;
            IsStrangerConnected = false;

            var handler = ServerClosedConnection;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnHeartbeatReceived(DateTime datetime)
        {
            var handler = HeartbeatReceived;
            if (handler != null) handler(this, datetime);
        }

        protected virtual void OnSocketClosed(string reason)
        {
            IsOpen = false;
            IsStrangerConnected = false;

            var handler = SocketClosed;

            if (handler != null)
                handler(this, reason);
        }

        protected virtual void OnSocketError(Exception e)
        {
            var handler = SocketError;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnSocketOpened()
        {
            IsOpen = true;

            var handler = SocketOpened;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion
    }
}
