using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObcyProtoRev.Protocol.SockJs
{
    class Packet : JObject
    {
        public string Header
        {
            get { return base["ev_name"].ToString(); }
            set { base["ev_name"] = value; }
        }

        public JToken Data
        {
            get { return base["ev_data"]; }
            set { base["ev_data"] = value; }
        }

        public Dictionary<string, JToken> AdditionalFields { get; set; }

        public Packet()
        {
            Header = "";
            AdditionalFields = new Dictionary<string, JToken>();
        }

        public Packet(string header)
            : this()
        {
            Header = header;
        }

        public Packet(string header, JToken data)
            : this(header)
        {
            Data = data;
        }

        public override string ToString()
        {
            var jArray = new JArray(ToString(Formatting.None));
            return jArray.ToString(Formatting.None);
        }
    }
}
