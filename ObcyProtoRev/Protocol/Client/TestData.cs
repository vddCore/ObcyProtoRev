namespace ObcyProtoRev.Protocol.Client
{
    public class TestData
    {
        public int CKey { get; }
        public bool RecevSent { get; }

        public TestData(int ckey, bool recevSent)
        {
            CKey = ckey;
            RecevSent = recevSent;
        }
    }
}