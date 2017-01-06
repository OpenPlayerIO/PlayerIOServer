using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class LoadObjectsError
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; }

        [ProtoMember(2)]
        public string Message { get; set; }
    }
}