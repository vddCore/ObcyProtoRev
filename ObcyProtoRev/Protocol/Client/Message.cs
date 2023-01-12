namespace ObcyProtoRev.Protocol.Client
{
    /// <summary>
    /// Represents a message received while a stranger is connected. This class cannot be inherited.
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        /// Gets a value describing a type of this message, look at <see cref="MessageType"/> for more information.
        /// </summary>
        public MessageType Type { get; private set; }

        /// <summary>
        /// Gets a value describing content of this message.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Gets a value describing ClientID of a person who sent this message.
        /// </summary>
        public int? ClientID { get; private set; }

        /// <summary>
        /// Gets a value describing PostID of this message.
        /// </summary>
        public int? PostID { get; private set; }

        /// <summary>
        /// Creates a new instance of Message class.
        /// </summary>
        /// <param name="type">A value describing type of this message.</param>
        /// <param name="body">A value describing content of this message.</param>
        /// <param name="clientId">A value describing ClientID of a person who sends this message.</param>
        /// <param name="postId">A value describing PostID of this message.</param>
        public Message(MessageType type, string body, int? clientId, int? postId)
        {
            Type = type;

            Body = body;
            ClientID = clientId;
            PostID = postId;
        }
    }
}
