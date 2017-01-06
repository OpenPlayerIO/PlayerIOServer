using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultBuyOutput
    {
        [ProtoMember(1)]
        public PayVaultBuyItemInfo VaultContents { get; set; }
    }
}