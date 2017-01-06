using System;
using Newtonsoft.Json;
using SecurityDriven.Inferno.Extensions;

namespace OpenPlayerIO.PlayerIOServer.Helpers
{
    public class JoinKey
    {
        public string ConnectUserId { get; set; }
        public string ServerType { get; set; }
        public string RoomId { get; set; }
        public long ExpiryTime { get; set; }

        public JoinKey()
        {

        }

        /// <summary> A PlayerToken with the expiry time set as 1 hour. </summary>
        public JoinKey(string connectUserId, string serverType, string roomId)
        {
            this.ConnectUserId = connectUserId;
            this.ServerType = serverType;
            this.RoomId = roomId;

            this.ExpiryTime = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        }

        public JoinKey(string connectUserId, DateTimeOffset expiryTime)
        {
            this.ConnectUserId = connectUserId;
            this.ExpiryTime = expiryTime.ToUnixTimeSeconds();
        }

        public string Encode()
        {
            return EncryptJoinKey(this).ToB64();
        }

        public static JoinKey Decode(string joinKey)
        {
            return DecryptJoinKey(joinKey.FromB64());
        }

        internal static byte[] EncryptJoinKey(JoinKey playerToken)
        {
            var serialized = JsonConvert.SerializeObject(playerToken);
            var encrypted = PlayerIOEncrypt.Encrypt(serialized.ToBytes());

            return encrypted;
        }

        internal static JoinKey DecryptJoinKey(byte[] joinKey, bool maskedToken = false)
        {
            var decrypted = PlayerIOEncrypt.Decrypt(joinKey);

            if (decrypted == null || decrypted.Length < 1)
                return null;

            return JsonConvert.DeserializeObject<JoinKey>(decrypted.FromBytes());
        }
    }
}
