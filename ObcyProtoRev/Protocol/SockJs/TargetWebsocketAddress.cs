namespace ObcyProtoRev.Protocol.SockJs
{
    class TargetWebsocketAddress
    {
        public int Port { get; }

        public string SocketNumber { get; }
        public string SocketSeed { get; }

        public string Origin => "http://6obcy.in";
        public string SocketIP => "server.6obcy.pl";

        public TargetWebsocketAddress()
        {
            Port = SocketGenerator.GeneratePortNumber();
            SocketNumber = SocketGenerator.GenerateRandomSocketNumber();
            SocketSeed = SocketGenerator.GenerateRandomSocketSeed(8);
        }

        public override string ToString()
        {
            return $"ws://{SocketIP}:{Port}/echoup/{SocketNumber}/{SocketSeed}/websocket";
        }

        public static implicit operator string(TargetWebsocketAddress twa)
        {
            return twa.ToString();
        }
    }
}
