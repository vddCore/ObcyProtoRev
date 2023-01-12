namespace ObcyProtoRev.Protocol.Client
{
    /// <summary>
    /// Represents information received at the moment of disconnect event. This class cannot be inherited.
    /// </summary>
    public sealed class DisconnectInfo
    {
        /// <summary>
        /// Gets a value indicating that disconnect event was fired outside a conversation.
        /// </summary>
        public bool IsReminder { get; private set; }

        /// <summary>
        /// Gets a value describing ClientID of stranger who caused the disconnect event to occur.
        /// </summary>
        public int ClientID { get; private set; }

        /// <summary>
        /// Creates a new instance of DisconnectInfo class.
        /// </summary>
        /// <param name="isReminder">Whether or not an event using this information is fired outside a conversation.</param>
        /// <param name="clientId">ClientID of a stranger who is causing the event to occur.</param>
        public DisconnectInfo(bool isReminder, int clientId)
        {
            IsReminder = isReminder;
            ClientID = clientId;
        }
    }
}
