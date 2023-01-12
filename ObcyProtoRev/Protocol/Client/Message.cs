namespace ObcyProtoRev.Protocol.Client
{
    public sealed class Message
    {
        public string Body { get; private set; }
        public int ClientID { get; private set; }
        public int PostID { get; private set; }

        public MessageType Type { get; private set; }

        public Message(string body, int clientId, int postId, MessageType type)
        {
            Body = body;
            ClientID = clientId;
            PostID = postId;

            Type = type;
        }
    }
}
