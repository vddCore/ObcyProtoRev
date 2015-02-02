namespace ObcyProtoRev.Protocol.Client
{
    public class UserAgent
    {
        public string Name { get; private set; }
        public string Version { get; private set; }

        public UserAgent(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public override string ToString()
        {
            return string.Format("{0}, version {1}");
        }
    }
}
