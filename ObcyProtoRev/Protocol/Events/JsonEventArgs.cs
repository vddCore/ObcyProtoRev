using System;

namespace ObcyProtoRev.Protocol.Events
{
    public class JsonEventArgs : EventArgs
    {
        public string JsonData { get; }

        public JsonEventArgs(string jsonData)
        {
            JsonData = jsonData;
        }
    }
}
