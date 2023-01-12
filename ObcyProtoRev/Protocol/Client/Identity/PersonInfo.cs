namespace ObcyProtoRev.Protocol.Client.Identity
{
    public class PersonInfo
    {
        public int Sex { get; private set; }
        public Location Location { get; private set; }

        public PersonInfo(int sex, Location location)
        {
            Sex = sex;
            Location = location;
        }
    }
}
