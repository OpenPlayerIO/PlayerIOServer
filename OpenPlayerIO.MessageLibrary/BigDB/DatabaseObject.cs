using System.Collections.Generic;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public partial class DatabaseObject
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public string Version { get; set; }

        [ProtoMember(3)]
        public Dictionary<string, BigDBObjectValue> Properties { get; set; }

        [ProtoMember(4)]
        public uint Creator { get; set; }
    }
}