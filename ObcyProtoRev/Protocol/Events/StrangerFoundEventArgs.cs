using System;
using ObcyProtoRev.Protocol.Client;

namespace ObcyProtoRev.Protocol.Events
{
    public class StrangerFoundEventArgs : EventArgs
    {
        public StrangerInfo StrangerInfo { get; }

        public StrangerFoundEventArgs(StrangerInfo strangerInfo)
        {
            StrangerInfo = strangerInfo;
        }
    }
}
