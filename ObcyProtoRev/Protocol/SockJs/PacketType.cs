namespace ObcyProtoRev.Protocol.SockJs
{
    public enum PacketType
    {
        ConnectionOpen,
        SocketMessage,
        BinaryData,
        ConnectionClose,
        SocketHeartbeat,
        Invalid
    }
}
