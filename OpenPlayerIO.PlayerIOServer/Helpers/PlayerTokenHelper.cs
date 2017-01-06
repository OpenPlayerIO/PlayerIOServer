using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SecurityDriven.Inferno.Extensions;

namespace OpenPlayerIO.PlayerIOServer.Helpers
{
    public static class PlayerTokenHelper
    {
        internal static byte[] MaskPlayerTokenBytes(string token, bool checkHeader = true) => MaskPlayerTokenBytes(token.ToBytes(), checkHeader);
        internal static byte[] MaskPlayerTokenBytes(byte[] token, bool checkHeader = true)
        {
            var output = new List<byte>();
            var length = BitConverter.GetBytes((ushort)token.Length).ToList();

            length.Reverse();

            output.Add((byte)(checkHeader ? 1 : 0));
            output.AddRange(length);
            output.AddRange(token);
            output.Add(1);

            return output.ToArray();
        }
    }
}