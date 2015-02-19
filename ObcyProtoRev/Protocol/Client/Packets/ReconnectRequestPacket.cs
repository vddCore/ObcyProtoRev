using System;
using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class ReconnectRequestPacket : Packet
    {
        private const string UaStringFormat = "{0}\"{1}\":true,\"version\":\"{2}\"{3}";

        public ReconnectRequestPacket(ReconnectInfo reconnectInfo)
        {
            Header = "_reconn_me";

            var connectionStateString = reconnectInfo.ClientState == ClientState.Chatting ? 
                                                                             "while conv" : 
                                                                             "out of conv";

            var time = (DateTime.Now - reconnectInfo.ConnectionTime).TotalMilliseconds;
            var uaString = string.Format(UaStringFormat, "{", reconnectInfo.UserAgent.Name, reconnectInfo.UserAgent.Version, "}");

            Data = new JObject();
            Data["log_msg"] = string.Format("{0}, time:{1} transports: websocket > websocket, browser: {2}", connectionStateString, time, uaString);
            Data["ckey"] = reconnectInfo.CurrentContactUID;
            Data["last_conn_id"] = reconnectInfo.LastConnectionUID;
            Data["last_post_id"] = reconnectInfo.LastPostID;
            Data["prev_ckey"] = reconnectInfo.PreviousContactUID;
            Data["mobile"] = reconnectInfo.IsMobile;
        }
    }
}
