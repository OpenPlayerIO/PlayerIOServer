using System.Collections.Generic;
using OpenPlayerIO.Messages.BigDB;
using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
    [ProtoContract]
    public class PayVaultItem
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string ItemKey { get; set; }

        [ProtoMember(3)]
        public long PurchaseDate { get; set; }

        [ProtoMember(4)]
        public KeyValuePair<string, BigDBObjectValue>[] Properties { get; set; }
    }
}