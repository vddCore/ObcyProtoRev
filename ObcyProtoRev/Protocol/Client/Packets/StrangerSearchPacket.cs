using Newtonsoft.Json.Linq;
using ObcyProtoRev.Protocol.Client.Identity;
using ObcyProtoRev.Protocol.SockJs;

namespace ObcyProtoRev.Protocol.Client.Packets
{
    public sealed class StrangerSearchPacket : Packet
    {
        public StrangerSearchPacket(PersonInfo myInfo, PersonInfo preferencesInfo, string channel)
        {
            Header = "_sas";

            var jObjectMyInfo = new JObject
            {
                ["sex"] = myInfo.Sex,
                ["loc"] = (int) myInfo.Location
            };

            var jObjectPreferencesInfo = new JObject
            {
                ["sex"] = preferencesInfo.Sex,
                ["loc"] = (int) preferencesInfo.Location
            };

            Data = new JObject
            {
                ["channel"] = channel,
                ["myself"] = jObjectMyInfo,
                ["preferences"] = jObjectPreferencesInfo,
                ["ceid"] = Connection.ActionID
            };
        }
    }
}
