using System.Collections.Generic;
using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultPaymentInfoOutput
    {
        [ProtoMember(1)]
        public KeyValuePair<string, string>[] ProviderArguments { get; set; }
    }
}