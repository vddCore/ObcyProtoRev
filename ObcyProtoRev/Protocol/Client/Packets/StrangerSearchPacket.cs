using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    /// <summary>
    /// Represents a stranger search request packet which is ready to be sent as-is. This class cannot be inherited.
    /// </summary>
    public sealed class StrangerSearchPacket : Packet
    {
        /// <summary>
        /// Creates a new instance of StrangerSearchPacket class.
        /// </summary>
        /// <param name="myInfo">A value describing sex and location of a client.</param>
        /// <param name="preferencesInfo">A value describing preferences for requested search.</param>
        /// <param name="channel">A channel to search in.</param>
        public StrangerSearchPacket(PersonInfo myInfo, PersonInfo preferencesInfo, string channel)
        {
            Header = "_sas";

            var jObjectMyInfo = new JObject();
            jObjectMyInfo["sex"] = myInfo.Sex;
            jObjectMyInfo["loc"] = (int)myInfo.Location;

            var jObjectPreferencesInfo = new JObject();
            jObjectPreferencesInfo["sex"] = preferencesInfo.Sex;
            jObjectPreferencesInfo["loc"] = (int)preferencesInfo.Location;

            Data = new JObject();
            Data["channel"] = channel;
            Data["myself"] = jObjectMyInfo;
            Data["preferences"] = jObjectPreferencesInfo;
        }
    }
}
