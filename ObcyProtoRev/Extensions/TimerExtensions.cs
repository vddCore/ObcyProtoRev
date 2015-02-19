using System.Timers;

namespace ObcyProtoRev.Extensions
{
    public static class TimerExtensions
    {
        public static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
