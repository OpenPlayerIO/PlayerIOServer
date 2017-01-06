using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class CreateObjectsArgs
    {
        [ProtoMember(1)]
        public SentDatabaseObject[] Objects { get; set; }

        [ProtoMember(2)]
        public bool LoadExisting { get; set; }
    }
}