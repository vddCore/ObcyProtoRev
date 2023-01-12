using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ObcyProtoRev.Protocol.SockJs
{
    static class SocketGenerator
    {
        private const int LowPortLimit = 7001;
        private const int HighPortLimit = 7017;

        private static readonly Random Rng = new Random();
        private static readonly List<int> PortBlacklist = new List<int>
        {
            7007,
            7009
        };

        public static int GeneratePortNumber()
        {
            var portNumber = Rng.Next(LowPortLimit, HighPortLimit);

            while(PortBlacklist.Contains(portNumber))
            {
                portNumber = Rng.Next(LowPortLimit, HighPortLimit);
            }
            return portNumber;
        }

        public static string GenerateRandomSocketNumber()
        {
            var sUid = Rng.Next();

            switch (sUid.ToString().Length)
            {
                case 1:
                    return sUid.ToString("00");
                case 2:
                    return sUid.ToString("0");
                default:
                    return sUid.ToString();
            }
        }

        public static string GenerateRandomSocketSeed(int length)
        {
            var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();

            crypto.GetNonZeroBytes(data);
            data = new byte[length];

            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(length);

            foreach (var b in data)
            {
                result.Append(characters[b % (characters.Length)]);
            }
            return result.ToString();
        }
    }
}
