using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultBuyArgs
    {
        [ProtoMember(1)]
        public string[] Items { get; set; }

        [ProtoMember(2)]
        public bool StoreItems { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }
}