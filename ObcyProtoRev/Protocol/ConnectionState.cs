namespace ObcyProtoRev.Protocol
{
    public enum ConnectionState
    {
        /// <summary>
        /// Describes a state when connection is open.
        /// </summary>
        Connected,

        /// <summary>
        /// Describes a state when connection is being open.
        /// </summary>
        Connecting,

        /// <summary>
        /// Describes a state when connection goes offline.
        /// </summary>
        Offline,

        /// <summary>
        /// Describes a state when connection is being revived.
        /// </summary>
        Reconnecting
    }
}
