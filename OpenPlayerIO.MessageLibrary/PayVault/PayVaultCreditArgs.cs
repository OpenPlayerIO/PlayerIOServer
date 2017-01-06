using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultCreditArgs
    {
        [ProtoMember(1)]
        public uint Amount { get; set; }

        [ProtoMember(2)]
        public string Reason { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }
}