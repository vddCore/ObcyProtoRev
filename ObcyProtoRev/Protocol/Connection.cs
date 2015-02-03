using System;
using ObcyProtoRev.Protocol.SockJs;
using WebSocketSharp;

namespace ObcyProtoRev.Protocol
{
    public class Connection
    {
        private WebSocket WebSocket { get; set; }
        private TargetWebsocketAddress WebsocketAddress { get; set; }

        public Connection()
        {
            RenewConnectionAddress();

            WebSocket = new WebSocket(WebsocketAddress)
            {
                Origin = WebsocketAddress.Origin
            };
            WebSocket.OnOpen += WebSocket_OnOpen;
            WebSocket.OnMessage += WebSocketOnOnMessage;
        }

        public void RenewConnectionAddress()
        {
            WebsocketAddress = new TargetWebsocketAddress();
        }

        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WebSocketOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
