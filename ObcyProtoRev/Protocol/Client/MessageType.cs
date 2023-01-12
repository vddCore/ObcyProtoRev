namespace ObcyProtoRev.Protocol.Client
{
    /// <summary>
    /// Specifies message types.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// A regular chat message.
        /// </summary>
        Chat,

        /// <summary>
        /// A service message, used for ads.
        /// </summary>
        Service,

        /// <summary>
        /// A random topic message.
        /// </summary>
        Topic
    }
}
