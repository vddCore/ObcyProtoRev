using System;

namespace ObcyProtoRev.Protocol.Events
{
    public class ConnectionAcceptedEventArgs : EventArgs
    {
        public string ConnectionID { get; }
        public string Hash { get; }

        public ConnectionAcceptedEventArgs(string connectionId, string hash)
        {
            ConnectionID = connectionId;
            Hash = hash;
        }
    }
}
