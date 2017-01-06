using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultConsumeOutput
    {
        [ProtoMember(1)]
        public PayVaultItem VaultContents { get; set; }
    }
}