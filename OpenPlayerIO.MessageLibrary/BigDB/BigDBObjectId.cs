using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class BigDBObjectId
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string[] Keys { get; set; }
    }
}