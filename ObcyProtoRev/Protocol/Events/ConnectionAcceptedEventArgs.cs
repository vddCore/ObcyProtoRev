using System;

namespace ObcyProtoRev.Protocol.Events
{
    public class ConnectionAcceptedEventArgs : EventArgs
    {
        public string ConnectionID { get; }

        public ConnectionAcceptedEventArgs(string connectionId)
        {
            ConnectionID = connectionId;
        }
    }
}
