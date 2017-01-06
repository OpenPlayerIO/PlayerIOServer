using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultRefreshArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }

        [ProtoMember(2)]
        public string TargetUserId { get; set; }
    }
}