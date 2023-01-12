using Newtonsoft.Json.Linq;

namespace ObcyProtoRev.Protocol.Client
{
    /// <summary>
    /// Represents stranger information at the moment of a new stranger search result. This class cannot be inherited.
    /// </summary>
    public sealed class ContactInfo
    {
        /// <summary>
        /// Gets a value describing ClientID assigned by the service.
        /// </summary>
        public int ClientID { get; private set; }

        /// <summary>
        /// Gets a value describing UID assigned by the service.
        /// </summary>
        public string UID { get; private set; }

        /// <summary>
        /// Gets a value describing stranger's preferences such as sex and preferred location.
        /// </summary>
        public JToken Preferences { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the stranger is marked as malicious/unpleasant.
        /// </summary>
        public bool FlaggedAsUnpleasant { get; private set; }

        /// <summary>
        /// Creates a new ContactInfo instance.
        /// </summary>
        /// <param name="clientId">Client ID assigned by the service.</param>
        /// <param name="uid">UID assigned by the service.</param>
        /// <param name="preferences">Stranger's preferences such as sex and preferred location.</param>
        /// <param name="flaggedAsUnpleasant">Value indicating whether or not the stranger is marked as malicious/unpleasant. True if they are, false otherwise.</param>
        public ContactInfo(int clientId, string uid, JToken preferences, bool flaggedAsUnpleasant)
        {
            ClientID = clientId;
            UID = uid;
            Preferences = preferences;
            FlaggedAsUnpleasant = flaggedAsUnpleasant;
        }
    }
}
