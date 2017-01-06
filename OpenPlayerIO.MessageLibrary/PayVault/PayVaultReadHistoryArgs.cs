using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultReadHistoryArgs
    {
        [ProtoMember(1)]
        public uint Page { get; set; }

        [ProtoMember(2)]
        public uint PageSize { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }
}