using System;
using ObcyProtoRev.Protocol.Client;

namespace ObcyProtoRev.Protocol.Events
{
    public class ConversationEndedEventArgs : EventArgs
    {
        public DisconnectInfo DisconnectInfo { get; }

        public ConversationEndedEventArgs(DisconnectInfo disconnectInfo)
        {
            DisconnectInfo = disconnectInfo;
        }
    }
}
