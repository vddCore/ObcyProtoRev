using System;
using ObcyProtoRev.Protocol.Client;

namespace ObcyProtoRev.Protocol.Events
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; }

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
