using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class LoadObjectsOutput
    {
        [ProtoMember(1)]
        public DatabaseObject[] Objects { get; set; }
    }
}