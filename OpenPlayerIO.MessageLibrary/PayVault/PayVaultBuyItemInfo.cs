using System.Collections.Generic;
using OpenPlayerIO.Messages.BigDB;
using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    public class PayVaultBuyItemInfo
    {
        [ProtoMember(1)]
        public string ItemKey { get; set; }

        [ProtoMember(2)]
        public KeyValuePair<string, BigDBObjectValue>[] Payload { get; set; }
    }
}