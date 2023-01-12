namespace ObcyProtoRev.Protocol.Client
{
    public class Message
    {
        public string Body { get; private set; }
        public int ClientId { get; private set; }
        public int PostId { get; private set; }

        public Message(string body, int clientId, int postId)
        {
            Body = body;
            ClientId = clientId;
            PostId = postId;
        }
    }
}
