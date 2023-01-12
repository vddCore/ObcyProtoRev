namespace ObcyProtoRev.Protocol.Client.Identity
{
    /// <summary>
    /// Represents application's identity sent to the server. This class cannot be inherited.
    /// </summary>
    public sealed class UserAgent
    {
        /// <summary>
        /// Gets a value describing application's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value describing application's version.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Creates a new UserAgent class instance.
        /// </summary>
        /// <param name="name">Application's name</param>
        /// <param name="version">Application's version string</param>
        public UserAgent(string name, string version)
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Converts current UserAgent instance to a properly formatted version string.
        /// </summary>
        /// <returns>String formatted as "ApplicationName, version X.X"</returns>
        public override string ToString()
        {
            return string.Format("{0}, version {1}", Name, Version);
        }
    }
}
