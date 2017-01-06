using System;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Extensions;

namespace OpenPlayerIO.PlayerIOServer.Helpers
{
    public static class PlayerIOEncrypt
    {
        public static byte[] MasterKey = "demo".ToBytes();
        public static CryptoRandom CRNG = new CryptoRandom();

        public static byte[] Encrypt(byte[] plaintext, ArraySegment<byte>? salt = null)
        {
            return SuiteB.Encrypt(MasterKey, new ArraySegment<byte>(plaintext), salt);
        }

        public static byte[] Decrypt(byte[] ciphertext, ArraySegment<byte>? salt = null)
        {
            return SuiteB.Decrypt(MasterKey, new ArraySegment<byte>(ciphertext), salt);
        }

        public static bool Authenticate(byte[] ciphertext, ArraySegment<byte>? salt = null)
        {
            return SuiteB.Authenticate(MasterKey, new ArraySegment<byte>(ciphertext), salt);
        }
    }
}
