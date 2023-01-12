using System;

namespace ObcyProtoRev.Protocol.Events
{
    public class SocketClosedEventArgs : EventArgs
    {
        public bool WasClean { get; }
        public int Code { get; }
        public string Reason { get; }

        public SocketClosedEventArgs(bool wasClean, int code, string reason)
        {
            WasClean = wasClean;
            Code = code;
            Reason = reason;
        }
    }
}
