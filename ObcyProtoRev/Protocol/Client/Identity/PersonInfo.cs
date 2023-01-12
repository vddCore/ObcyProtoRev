namespace ObcyProtoRev.Protocol.Client.Identity
{
    /// <summary>
    /// Represents client information used in stranger search requests.
    /// </summary>
    public class PersonInfo
    {
        /// <summary>
        /// Gets a value describing sex preference.
        /// </summary>
        public int Sex { get; private set; }

        /// <summary>
        /// Gets a value describing location preference.
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Creates a new instance of PersonInfo class.
        /// </summary>
        /// <param name="sex">Value describing sex preference.</param>
        /// <param name="location">Value describing location preference.</param>
        public PersonInfo(int sex, Location location)
        {
            Sex = sex;
            Location = location;
        }
    }
}
