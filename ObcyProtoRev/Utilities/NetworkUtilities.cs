using System.Diagnostics;
using System.Net.NetworkInformation;

namespace ObcyProtoRev.Utilities
{
    public static class NetworkUtilities
    {
        private static int retries;

        public static bool InternetConnectionAvailable(int timeout)
        {
            while (true)
            {
                var ping = new Ping();
                PingReply reply = ping.Send("8.8.8.8", timeout);

                Debug.Assert(reply != null, "INetConnectionAvailable: reply != null");

                if (reply.Status != IPStatus.Success)
                {
                    if (retries >= 3)
                    {
                        retries = 0;
                        return false;
                    }
                    retries++;
                }
                else
                {
                    retries = 0;
                    return true;
                }
            }
        }
    }
}
