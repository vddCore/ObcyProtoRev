using Newtonsoft.Json.Linq;

namespace ObcyProtoRev.Protocol.Client
{
    public class ContactInfo
    {
        public int ClientID { get; private set; }
        public string UID { get; private set; }
        public JToken Preferences { get; private set; }
        public bool FlaggedAsUnpleasant { get; private set; }

        public ContactInfo(int clientId, string uid, JToken preferences, bool flaggedAsUnpleasant)
        {
            ClientID = clientId;
            UID = uid;
            Preferences = preferences;
            FlaggedAsUnpleasant = flaggedAsUnpleasant;
        }
    }
}
