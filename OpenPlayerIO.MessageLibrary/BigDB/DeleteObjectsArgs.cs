using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class DeleteObjectsArgs
    {
        [ProtoMember(1)]
        public string[] ObjectIds { get; set; }
    }
}