using ProtoBuf;

namespace OpenPlayerIO.Messages.Construct
{
    [ProtoContract]
    public class KeyValuePair
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public string Value { get; set; }
    }
}