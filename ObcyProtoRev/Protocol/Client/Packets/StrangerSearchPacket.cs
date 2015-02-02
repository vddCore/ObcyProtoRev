using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    class StrangerSearchPacket : Packet
    {
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
