using ProtoBuf;

namespace OpenPlayerIO.Messages.Error
{
    [ProtoContract]
    public class Error
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; }

        [ProtoMember(2)]
        public string Message { get; set; }
    }
}