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
    /// <summary>
    /// Exposes methods, events and fields used to communicate with 6Obcy service. 
    /// </summary>
    public class Connection
    {
        #region Private fields
        /// <summary>
        /// Gets or sets a value exposing connection's underlying websocket.
        /// </summary>
        private WebSocket WebSocket { get; set; }

        /// <summary>
        /// Gets or sets a value exposing current generated target websocket address.
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
        /// Gets or sets a value indicating whether a connection should report itself as mobile.
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
        /// Gets or sets a value indicating whether the connection should be kept alive automatically.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection should send its identity.
        /// </summary>
        public bool SendUserAgent { get; set; }

        /// <summary>
        /// Gets a value indicating current stranger UID assigned by the service.
        /// </summary>
        public string CurrentContactUID { get; private set; }

        /// <summary>
        /// Gets or sets a value describing an identity that should be sent on connection acknowledge.
        /// </summary>
        public UserAgent UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a value describing packet's action ID.
        /// </summary>
        public static int ActionID { get; set; }
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
        /// Event that gets fired when the client sents a message to the currently connected stranger.
        /// </summary>
        public event MessageEventHandler MessageSent;

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
            UserAgent = new UserAgent("", "v2.5");
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

                var di = new DisconnectInfo(false, 0);
                ConversationEnded?.Invoke(this, di);

                ActionID++;
            }
        }

        /// <summary>
        /// Reports currently or last connected stranger as malicious.
        /// </summary>
        public void FlagStranger()
        {
            if (IsOpen && IsReady)
            {
                SendPacket(new ReportStrangerPacket(CurrentContactUID));
                ActionID++;
            }
        }

        /// <summary>
        /// Sends a "pong" packet to the server.
        /// </summary>
        public void PongResponse()
        {
            if (IsOpen && IsReady)
            {
                SendPacket(new PongPacket());
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
                SendPacket(new ChatstatePacket(isTyping, CurrentContactUID));
            }
        }

        /// <summary>
        /// Sends a request to the service for a random topic shuffle.
        /// </summary>
        public void RequestRandomTopic()
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                SendPacket(new RandomTopicPacket(CurrentContactUID));
                ActionID++;
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
                    SendPacket(new StrangerSearchPacket(info, info, "main"));
                }
                ActionID++;
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
                SendPacket(new MessagePacket(message, CurrentContactUID));
                
                MessageSent?.Invoke(this, new Message(message, null, null, MessageType.Chat));
                ActionID++;
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

            JsonWritten?.Invoke(this, packet);
        }

        /// <summary>
        /// Sends a free-form, unsafe packet in JSON format.
        /// </summary>
        /// <param name="json">JSON packet string to be sent.</param>
        /// <remarks>Make sure the packet is WELL formed. Otherwise client will certainly get disconnected.</remarks>
        public void SendJson(string json)
        {
            if (IsReady && IsOpen)
                WebSocket.Send(json);

            JsonWritten?.Invoke(this, json);
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
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    SendPacket(new ClientInfoPacket(false, UserAgent));
                    SendPacket(new OpenAcknowledgedPacket());
                    ConnectionAccepted?.Invoke(this, packet.Data["conn_id"].ToString());
                    continue;
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
                        var di = new DisconnectInfo(true, int.Parse(packet.Data.ToString()));
                        ConversationEnded?.Invoke(this, di);
                    }
                    else
                    {
                        var di = new DisconnectInfo(true, 0);
                        ConversationEnded?.Invoke(this, di);
                    }
                    IsStrangerConnected = false;
                    continue;
                }

                if (packet.Header == StrangerDisconnectedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    var di = new DisconnectInfo(false, int.Parse(packet.Data.ToString()));
                    ConversationEnded?.Invoke(this, di);
                    IsStrangerConnected = false;
                }

                if (packet.Header == MessageReceivedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    int postId = -1;
                    if (packet.AdditionalFields.ContainsKey("post_id"))
                        postId = int.Parse(packet.AdditionalFields["post_id"].ToString());

                    var message = new Message(
                        packet.Data["msg"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        postId,
                        MessageType.Chat
                    );
                    MessageReceived?.Invoke(this, message);
                    continue;
                }

                if (packet.Header == OnlinePeopleCountPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    OnlinePeopleCountChanged?.Invoke(this, int.Parse(packet.Data.ToString()));
                    continue;
                }

                if (packet.Header == PingPacket.ToString())
                {
                    if (KeepAlive)
                        PongResponse();

                    PingReceived?.Invoke(this, DateTime.Now);
                    continue;
                }

                if (packet.Header == RandomTopicReceivedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    var message = new Message(
                        packet.Data["topic"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.AdditionalFields["post_id"].ToString()),
                        MessageType.Topic
                    );
                    MessageReceived?.Invoke(this, message);
                    continue;
                }

                if (packet.Header == ServiceMessageReceivedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    var message = new Message(packet.Data.ToString(), null, null, MessageType.Service);
                    MessageReceived?.Invoke(this, message);
                    continue;
                }

                if (packet.Header == StrangerChatstatePacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    StrangerChatstateChanged?.Invoke(this, bool.Parse(packet.Data.ToString()));
                    continue;
                }

                if (packet.Header == StrangerFoundPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    CurrentContactUID = packet.Data["ckey"].ToString();

                    IsSearchingForStranger = false;
                    IsStrangerConnected = true;

                    var ci = new ContactInfo(
                        int.Parse(packet.Data["cid"].ToString()),
                        packet.Data["ckey"].ToString(),
                        bool.Parse(packet.Data["flaged"].ToString()),
                        packet.Data["info"]
                    );
                    StrangerFound?.Invoke(this, ci);
                }
            }
        }

        private void WebsocketPacketHandlerOnSocketHeartbeatReceived(DateTime heartbeatTime)
        {
            HeartbeatReceived?.Invoke(this, heartbeatTime);
        }

        private void WebsocketPacketHandlerOnConnectionClosePacketReceived(EventArgs e)
        {
            IsOpen = false;
            IsStrangerConnected = false;

            ServerClosedConnection?.Invoke(this, e);
        }

        private void WebsocketPacketHandler_ConnectionOpenPacketReceived(EventArgs e)
        {
            ConnectionAcknowledged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Low-level websocket events
        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            IsOpen = true;
            SocketOpened?.Invoke(this, e);
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            JsonRead?.Invoke(this, messageEventArgs.Data);
            WebsocketPacketHandler.HandlePacket(messageEventArgs.Data);
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            SocketError?.Invoke(this, e.Exception);
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            IsOpen = false;
            IsStrangerConnected = false;

            SocketClosed?.Invoke(this, e.Reason);
        }
        #endregion
    }
}
