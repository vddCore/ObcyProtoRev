using Newtonsoft.Json.Linq;

namespace ObcyProtoRev.Protocol.Client
{
    /// <summary>
    /// Class responsible for storing currently connected contact information.
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// Gets ClientID assigned by the service.
        /// </summary>
        public int ClientID { get; private set; }

        /// <summary>
        /// Gets UID assigned by the service.
        /// </summary>
        public string UID { get; private set; }

        /// <summary>
        /// Gets stranger's preferences such as sex and preferred location.
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
