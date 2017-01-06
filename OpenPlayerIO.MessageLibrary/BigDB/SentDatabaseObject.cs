using System.Collections.Generic;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public partial class SentDatabaseObject
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Key { get; set; }

        [ProtoMember(3)]
        public Dictionary<string, BigDBObjectValue> Properties { get; set; }
    }
}
