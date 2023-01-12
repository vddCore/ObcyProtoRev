using Newtonsoft.Json.Linq;

namespace ObcyProtoRev.Protocol.Client
{
    public sealed class StrangerInfo
    {

        public int ClientID { get; private set; }
        public string UID { get; private set; }
        public bool FlaggedAsUnpleasant { get; private set; }

        public JToken Preferences { get; private set; }

        public StrangerInfo(int clientId, string uid, bool flaggedAsUnpleasant, JToken preferences)
        {
            ClientID = clientId;
            UID = uid;
            FlaggedAsUnpleasant = flaggedAsUnpleasant;
            Preferences = preferences;
        }
    }
}
