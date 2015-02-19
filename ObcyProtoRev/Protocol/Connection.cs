using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

using ObcyProtoRev.Extensions;
using ObcyProtoRev.Protocol.Client;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.Client.Packets;
using ObcyProtoRev.Protocol.Server.Packets;
using ObcyProtoRev.Protocol.SockJs;
using ObcyProtoRev.Utilities;
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

        /// <summary>
        /// Gets or sets the reconnect attempt timer.
        /// </summary>
        private Timer ReconnectTimer { get; set; }

        /// <summary>
        /// Gets or sets the value indicating server's ping failure count in case of severed connection.
        /// </summary>
        private int NoPingCounter { get; set; }

        /// <summary>
        /// Gets or sets the value describing previous stranger ID.
        /// </summary>
        private string PreviousContactUID { get; set; }

        /// <summary>
        /// Gets or sets the value describing last message ID.
        /// </summary>
        private int LastPostID { get; set; }
        #endregion

        #region Public fields
        /// <summary>
        /// Gets a value indicating current client state.
        /// </summary>
        public ClientState ClientState { get; private set; }

        /// <summary>
        /// Gets a value indicating current connection state.
        /// </summary>
        public ConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current session has made a connection at least once.
        /// </summary>
        public bool HasEverSearchedForStranger { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether a connection should report itself as mobile.
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        /// Gets a value indicating that socket is ready to open a connection.
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Gets or sets whether the connection should be kept alive automatically.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets whether the connection should send its identity.
        /// </summary>
        public bool SendUserAgent { get; set; }

        /// <summary>
        /// Gets a value indicating current stranger UID sent by the service.
        /// </summary>
        public string CurrentContactUID { get; private set; }

        /// <summary>
        /// Gets a value indicating your current connection UID assigned by the service.
        /// </summary>
        public string CurrentConnectionUID { get; private set; }
        
        /// <summary>
        /// Gets a value describing the last socket opening time.
        /// </summary>
        public DateTime LastConnectionTime { get; private set; }

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
        public delegate void StringEventHandler(object sender, string value);
        #endregion

        #region Event handlers
        /// <summary>
        /// Event that gets fired after the client receives "ConnectionAccepted" service packet.
        /// </summary>
        public event EventHandler ConnectionAccepted;

        /// <summary>
        /// Event that gets fired after the client receives the meaningful "o" socket packet.
        /// </summary>
        public event EventHandler ConnectionAcknowledged;

        /// <summary>
        /// Event that gets fired when a stranger disconnects or client tries to send a packet that requires an active conversation.
        /// </summary>
        public event ConversationEndEventHandler ConversationEnded;

        /// <summary>
        /// Event that gets fired when the connection is lost.
        /// </summary>
        public event EventHandler ConnectionLost;

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
        public event EventHandler ServerClosedConnection;

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
        public event EventHandler SocketOpened;

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
            ConnectionState = ConnectionState.Offline;
            ClientState = ClientState.Idle;

            RenewConnectionAddress();
            CreateWebsocket();
            RegisterPacketHandlerEvents();

            PreviousContactUID = "0";
            HasEverSearchedForStranger = false;

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
            if (ConnectionState == ConnectionState.Connected)
                WebSocket.Close();
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void Open()
        {
            if (IsReady && ConnectionState == ConnectionState.Offline)
                WebSocket.Connect();
        }

        /// <summary>
        /// Disconnects from currently connected stranger.
        /// </summary>
        public void DisconnectStranger()
        {
            if (ConnectionState == ConnectionState.Connected && ClientState == ClientState.Chatting)
            {
                SendPacket(
                    new DisconnectPacket(CurrentContactUID)
                );
                ClientState = ClientState.Idle;

                DisconnectInfo di = new DisconnectInfo(false, 0);
                OnConversationEnded(di);
            }
        }

        /// <summary>
        /// Reports currently or last connected stranger as malicious.
        /// </summary>
        public void FlagStranger()
        {
            if (ConnectionState == ConnectionState.Connected)
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
            if (ConnectionState == ConnectionState.Connected)
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
            if (ConnectionState == ConnectionState.Connected && ClientState == ClientState.Chatting)
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
            if (ConnectionState == ConnectionState.Connected && ClientState == ClientState.Chatting)
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
            if (ConnectionState == ConnectionState.Connected && ClientState == ClientState.Idle)
            {
                if (ClientState == ClientState.Idle)
                {
                    ClientState = ClientState.SearchingForStranger;
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
            if (ConnectionState == ConnectionState.Connected && ClientState == ClientState.Chatting)
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
            if (ConnectionState == ConnectionState.Connected || ConnectionState == ConnectionState.Reconnecting)
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
            if (ConnectionState == ConnectionState.Connected)
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

        private void CreateAndStartReconnectTimer()
        {
            // 30 seconds for ping receive + 5 reserved seconds just in case.
            ReconnectTimer = new Timer(35000);
            ReconnectTimer.Elapsed += ReconnectTimer_Elapsed;

            ReconnectTimer.Start();
        }

        private void SendReconnectRequest()
        {
            var reconnectInfo = new ReconnectInfo(
                        IsMobile,
                        ClientState,
                        LastConnectionTime,
                        LastPostID,
                        PreviousContactUID,
                        CurrentContactUID,
                        CurrentConnectionUID,
                        UserAgent
                    );

            var reconnectRequestPacket = new ReconnectRequestPacket(reconnectInfo);
            SendPacket(reconnectRequestPacket);
        }
        #endregion

        #region Reconnect timer stuff
        private void ReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (NoPingCounter < 1)
            {
                NoPingCounter++;
            }
            else
            {
                if (ConnectionState != ConnectionState.Reconnecting)
                {
                    ConnectionState = ConnectionState.Reconnecting;
                    OnConnectionLost();
                }
                AttemptReconnect();
            }
        }

        private void AttemptReconnect()
        {
            WebSocket.Close();

            if (NetworkUtilities.InternetConnectionAvailable(2000))
            {
                RenewConnectionAddress();
                CreateWebsocket();

                WebSocket.Connect();
            }
        }

        #endregion

        #region Packet handler events
        private void WebsocketPacketHandler_SocketMessageReceived(List<Packet> packets)
        {
            foreach (var packet in packets)
            {
                if (PacketHelper.IsConnectionAcceptedPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "ConnectionAccepted: packet.Data != null");

                    if (ConnectionState == ConnectionState.Reconnecting)
                    {
                        SendReconnectRequest();
                        return;
                    }

                    CurrentConnectionUID = packet.Data["conn_id"].ToString();
                    CreateAndStartReconnectTimer();
                    OnConnectionAccepted();
                }

                if (PacketHelper.IsConversationEndedPacket(packet))
                {
                    // Unusual behavior, server sends "convended" without any data
                    // if "flag stranger" packet is sent and no conversation have
                    // been started before.
                    //
                    // Hence, we have to handle it like this.

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

                if (PacketHelper.IsStrangerDisconnectedPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "StrangerDisconnected: packet.Data != null");
                    PreviousContactUID = CurrentContactUID;

                    OnConversationEnded(
                        new DisconnectInfo(
                            false,
                            int.Parse(packet.Data.ToString())
                        )
                    );
                }

                if (PacketHelper.IsMessageReceivedPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "MessageReceived: packet.Data != null");

                    var message = new Message(
                        MessageType.Instant,
                        packet.Data["msg"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.AdditionalFields["post_id"].ToString())
                    );
                    LastPostID = int.Parse(packet.AdditionalFields["post_id"].ToString());

                    OnMessageReceived(message);
                }
                    
                if (PacketHelper.IsOnlinePeopleCountPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "OnlineCountChanged: packet.Data != null");
                     
                    OnOnlinePeopleCountChanged(
                        int.Parse(packet.Data.ToString())
                    );
                }

                if (PacketHelper.IsPingPacket(packet))
                {
                    Debug.Assert(packet.Data == null, "PingReceived: packet.Data == null");

                    // Extension method, see ObcyProtoRev.Extensions.TimerExtensions
                    ReconnectTimer.Reset();

                    if (KeepAlive)
                        PongResponse();

                    OnPingReceived(DateTime.Now);
                }

                if (PacketHelper.IsRandomTopicReceivedPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "RandomTopicReceived: packet.Data != null");

                    var message = new Message(
                        MessageType.Subject,
                        packet.Data["topic"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.AdditionalFields["post_id"].ToString())
                    );
                    LastPostID = int.Parse(packet.AdditionalFields["post_id"].ToString());

                    OnMessageReceived(message);
                }

                if (PacketHelper.IsReconnectionSuccessPacket(packet))
                {
                    ConnectionState = ConnectionState.Connected;
                }

                if (PacketHelper.IsServiceMessageReceivedPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "ServiceMessageReceived: packet.Data != null");

                    var message = new Message(MessageType.Service, packet.Data.ToString(), null, null);
                    OnMessageReceived(message);
                }

                if (PacketHelper.IsStrangerChatstatePacket(packet))
                {
                    Debug.Assert(packet.Data != null, "StrangerChatstateChanged: packet.Data != null");

                    OnStrangerChatstateChanged(
                        bool.Parse(packet.Data.ToString())
                    );
                }

                if (PacketHelper.IsStrangerFoundPacket(packet))
                {
                    Debug.Assert(packet.Data != null, "StrangerFound: packet.Data != null");

                    CurrentContactUID = packet.Data["ckey"].ToString();

                    // Reliability fix to ensure we find a proper stranger upon reconnection to the FIRST
                    // stranger in the current session. Oh, look - their webclient fucked it up.
                    if (!HasEverSearchedForStranger)
                    {
                        PreviousContactUID = CurrentContactUID;
                        HasEverSearchedForStranger = true;
                    }

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
            LastConnectionTime = DateTime.Now;

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
        protected virtual void OnConnectionAccepted()
        {
            var handler = ConnectionAccepted;

            if (handler != null)
                handler(this, EventArgs.Empty);

        }

        protected virtual void OnConnectionLost()
        {
            var handler = ConnectionLost;
            if (handler != null) handler(this, EventArgs.Empty);
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
            ClientState = ClientState.Chatting;

            var handler = StrangerFound;

            if (handler != null)
                handler(this, contactinfo);
        }

        protected virtual void OnConversationEnded(DisconnectInfo disconnectInfo)
        {
            ClientState = ClientState.Idle;

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
            ConnectionState = ConnectionState.Offline;
            ClientState = ClientState.Idle;

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
            if (ConnectionState != ConnectionState.Reconnecting)
            {
                ConnectionState = ConnectionState.Offline;
                ClientState = ClientState.Idle;
            }

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
            if (ConnectionState != ConnectionState.Reconnecting)
            {
                ConnectionState = ConnectionState.Connected;
            }

            var handler = SocketOpened;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion
    }
}
