using ObcyProtoRev.Protocol.Client;

namespace ObcyProtoRev.Protocol.Events
{
    public class ChatstateEventArgs
    {
        public ChatState ChatState { get; }

        public ChatstateEventArgs(ChatState chatState)
        {
            ChatState = chatState;
        }
    }
}
