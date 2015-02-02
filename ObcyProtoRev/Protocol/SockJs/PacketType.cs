namespace ObcyProtoRev.Protocol.SockJs
{
    public enum PacketType
    {
        ConnectionOpen,
        ConnectionClose,
        SocketHeartbeat,
        SocketMessage,
        BinaryData,
        Invalid
    }
}
