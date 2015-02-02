using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObcyProtoRev.Protocol.SockJs
{
    static class Decoder
    {
        public static PacketType DeterminePacketType(string sockJsPacket)
        {
            switch (sockJsPacket[0])
            {
                case 'o':
                    return PacketType.ConnectionOpen;
                case 'c':
                    return PacketType.ConnectionClose;
                case 'h':
                    return PacketType.SocketHeartbeat;
                case 'a':
                    return PacketType.SocketMessage;
                case 'm':
                    return PacketType.BinaryData;
                default:
                    return PacketType.Invalid;
            }
        }

        public static List<Packet> DecodePackets(string sockJsPacket)
        {
            if (DeterminePacketType(sockJsPacket) != PacketType.SocketMessage)
                return new List<Packet>();

            var packets = new List<Packet>();

            var jStrings = JsonConvert.DeserializeObject<List<string>>(
                StripPacketHeader(sockJsPacket)
            );
            
            var jObjects = jStrings.Select(
                JsonConvert.DeserializeObject<JObject>
            );

            foreach (var obj in jObjects)
            {
                var packet = new Packet();
                foreach (var token in obj)
                {
                    switch (token.Key)
                    {
                        case "ev_name":
                            packet.Header = token.Value.ToString(Formatting.None).Replace("\"", "");
                            break;
                        case "ev_data":
                            packet.Data = token.Value;
                            break;
                        default:
                            packet.AdditionalFields.Add(token.Key, token.Value);
                            break;
                    }
                }
                packets.Add(packet);
            }
            return packets;
        }

        private static string StripPacketHeader(string sockJsPacket)
        {
            return sockJsPacket.Substring(1);
        }
    }
}
