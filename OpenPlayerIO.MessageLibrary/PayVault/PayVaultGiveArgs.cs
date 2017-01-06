using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultGiveArgs
    {
        [ProtoMember(1)]
        public PayVaultBuyItemInfo[] Items { get; set; }

        [ProtoMember(2)]
        public string TargetUserId { get; set; }
    }
}