using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class CreateObjectsOutput
    {
        [ProtoMember(1)]
        public DatabaseObject[] Objects { get; set; }
    }
}