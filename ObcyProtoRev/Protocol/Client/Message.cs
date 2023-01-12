namespace ObcyProtoRev.Protocol.Client
{
    public class Message
    {
        public MessageType Type { get; private set; }

        public string Body { get; private set; }
        public int ClientId { get; private set; }
        public int PostId { get; private set; }

        public Message(MessageType type, string body, int clientId, int postId)
        {
            Type = type;

            Body = body;
            ClientId = clientId;
            PostId = postId;
        }
    }
}
