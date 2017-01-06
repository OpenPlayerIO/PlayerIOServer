using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultConsumeArgs
    {
        [ProtoMember(1)]
        public string[] Ids { get; set; }

        [ProtoMember(2)]
        public string TargetUserId { get; set; }
    }
}