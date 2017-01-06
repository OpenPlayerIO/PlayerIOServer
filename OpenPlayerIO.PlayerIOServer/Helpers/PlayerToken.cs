using System;
using Newtonsoft.Json;
using SecurityDriven.Inferno.Extensions;

namespace OpenPlayerIO.PlayerIOServer.Helpers
{
    public enum PlayerTokenState { Valid, Invalid, Expired }
    public class PlayerToken
    {
        public string ConnectUserId { get; set; }
        public string GameId { get; set; }
        public long ExpiryTime { get; set; }

        private bool Invalid { get; set; } = false;
        private bool Expired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() > this.ExpiryTime;

        public PlayerTokenState State => (this.Invalid) ? PlayerTokenState.Invalid :
                                         (this.Expired) ? PlayerTokenState.Expired :
                                                          PlayerTokenState.Valid;

        public PlayerToken()
        {

        }

        public PlayerToken(string gameId, string connectUserId)
        {
            this.GameId = gameId;
            this.ConnectUserId = connectUserId;
            this.ExpiryTime = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();
        }

        public PlayerToken(string gameId, string connectUserId, DateTimeOffset expiryTime)
        {
            this.GameId = gameId;
            this.ConnectUserId = connectUserId;
            this.ExpiryTime = expiryTime.ToUnixTimeSeconds();
        }

        public string Encode()
        {
            return EncryptPlayerToken(this).ToB64();
        }

        public static PlayerToken Decode(string playerToken)
        {
            return DecryptPlayerToken(playerToken.FromB64());
        }

        private static byte[] EncryptPlayerToken(PlayerToken playerToken)
        {
            var serialized = JsonConvert.SerializeObject(playerToken);
            var encrypted = PlayerIOEncrypt.Encrypt(serialized.ToBytes());

            return encrypted;
        }

        private static PlayerToken DecryptPlayerToken(byte[] playerToken, bool maskedToken = false)
        {
            if (!PlayerIOEncrypt.Authenticate(playerToken))
                return new PlayerToken() { Invalid = true };

            var decrypted = PlayerIOEncrypt.Decrypt(playerToken).FromBytes();
            var deserialized = JsonConvert.DeserializeObject<PlayerToken>(decrypted);

            return deserialized;
        }
    }
}
