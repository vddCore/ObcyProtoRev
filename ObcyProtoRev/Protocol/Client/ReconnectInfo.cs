using System;

namespace ObcyProtoRev.Protocol.Client
{
    public class ReconnectInfo
    {
        /// <summary>
        /// Gets a value indicating whether request is being sent from a mobile device.
        /// </summary>
        public bool IsMobile { get; private set; }

        /// <summary>
        /// Gets a value describing client state at the time of connection loss.
        /// </summary>
        public ClientState ClientState { get; private set; }

        /// <summary>
        /// Gets a value describing session creation time.
        /// </summary>
        public DateTime ConnectionTime { get; private set; }

        /// <summary>
        /// Gets a value describing last message ID.
        /// </summary>
        public int LastPostID { get; private set; }

        /// <summary>
        /// Gets a value describing previous stranger UID.
        /// </summary>
        public string PreviousContactUID { get; private set; }

        /// <summary>
        /// Gets a value describing current stranger UID.
        /// </summary>
        public string CurrentContactUID { get; private set; }

        /// <summary>
        /// Gets a value describing last assigned UID.
        /// </summary>
        public string LastConnectionUID { get; private set; }

        /// <summary>
        /// Gets a value describing UserAgent used at the time of connection loss.
        /// </summary>
        public UserAgent UserAgent { get; private set; }

        public ReconnectInfo(bool isMobile, ClientState clientState, DateTime connectionTime, int lastPostID, string prevContactUID, string currentContactUID, string lastConnectionUID, UserAgent userAgent)
        {
            IsMobile = isMobile;
            ClientState = clientState;
            ConnectionTime = connectionTime;
            LastPostID = lastPostID;
            PreviousContactUID = prevContactUID;
            CurrentContactUID = currentContactUID;
            LastConnectionUID = lastConnectionUID;
            UserAgent = userAgent;
        }
    }
}
