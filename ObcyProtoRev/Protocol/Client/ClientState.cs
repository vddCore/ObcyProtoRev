namespace ObcyProtoRev.Protocol.Client
{
    public enum ClientState
    {
        /// <summary>
        /// Describes a state when stranger is connected.
        /// </summary>
        Chatting,

        /// <summary>
        /// Describes a state when client is neither connected nor searching for any stranger.
        /// </summary>
        Idle,

        /// <summary>
        /// Describes a state when client is currently searching for a stranger.
        /// </summary>
        SearchingForStranger
    }
}
