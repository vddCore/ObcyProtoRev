namespace ObcyProtoRev.Protocol.SockJs
{
    class TargetWebsocketAddress
    {
        public int Port { get; private set; }

        public string SocketNumber { get; private set; }
        public string SocketSeed { get; private set; }

        public string Origin
        {
            get { return "http://6obcy.in"; }
        }

        public string SocketIP
        {
            get { return "91.185.186.211"; }
        }

        public TargetWebsocketAddress()
        {
            Port = SocketGenerator.GeneratePortNumber();
            SocketNumber = SocketGenerator.GenerateRandomSocketNumber();
            SocketSeed = SocketGenerator.GenerateRandomSocketSeed(8);
        }

        public override string ToString()
        {
            return string.Format(
                "ws://{0}:{1}/echoup/{2}/{3}/websocket",
                SocketIP,
                Port,
                SocketNumber,
                SocketSeed
            );
        }

        public static implicit operator string(TargetWebsocketAddress twa)
        {
            return twa.ToString();
        }
    }
}
