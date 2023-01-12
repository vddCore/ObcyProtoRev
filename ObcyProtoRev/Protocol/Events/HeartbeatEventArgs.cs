using System;

namespace ObcyProtoRev.Protocol.Events
{
    public class HeartbeatEventArgs : EventArgs
    {
        public DateTime ReceivedDate { get; }

        public HeartbeatEventArgs(DateTime receivedDate)
        {
            ReceivedDate = receivedDate;
        }
    }
}
