using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using OpenPlayerIO.Messages.Enums;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public partial class BigDBObjectValue
    {
        [ProtoMember(1)]
        public ObjectType Type { get; set; }

        [ProtoMember(2), JsonIgnore, BsonIgnore]
        public string ValueString { get; set; }

        [ProtoMember(3), JsonIgnore, BsonIgnore]
        public int ValueInteger { get; set; }

        [ProtoMember(4), JsonIgnore, BsonIgnore]
        public uint ValueUInteger { get; set; }

        [ProtoMember(5), JsonIgnore, BsonIgnore]
        public long ValueLong { get; set; }

        [ProtoMember(6), JsonIgnore, BsonIgnore]
        public bool ValueBoolean { get; set; }

        [ProtoMember(7), JsonIgnore, BsonIgnore]
        public float ValueFloat { get; set; }

        [ProtoMember(8), JsonIgnore, BsonIgnore]
        public double ValueDouble { get; set; }

        [ProtoMember(9), JsonIgnore, BsonIgnore]
        public byte[] ValueByteArray { get; set; }

        [ProtoMember(10), JsonIgnore, BsonIgnore]
        public long ValueDateTime { get; set; }

        [ProtoMember(11), JsonIgnore, BsonIgnore]
        public KeyValuePair<int, BigDBObjectValue>[] ValueArray { get; set; }

        [ProtoMember(12), JsonIgnore, BsonIgnore]
        public KeyValuePair<string, BigDBObjectValue>[] ValueObject { get; set; }
    }
}