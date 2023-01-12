using System;

namespace ObcyProtoRev.Protocol.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public Exception Exception { get; }

        public ErrorEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}
