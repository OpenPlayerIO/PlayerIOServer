using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class LoadObjectsArgs
    {
        [ProtoMember(1)]
        public BigDBObjectId ObjectIds { get; set; }
    }
}