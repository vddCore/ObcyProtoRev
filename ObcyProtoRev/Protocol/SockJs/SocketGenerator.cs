using System;
using System.Security.Cryptography;
using System.Text;

namespace ObcyProtoRev.Protocol.SockJs
{
    static class SocketGenerator
    {
        public static string GenerateRandomSocketNumber()
        {
            var sUid = new Random().Next(0, 999);

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
