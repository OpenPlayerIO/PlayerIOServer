using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultGiveOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}