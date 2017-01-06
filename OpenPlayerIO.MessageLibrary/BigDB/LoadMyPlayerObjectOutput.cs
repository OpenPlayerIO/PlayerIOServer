using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    [ProtoContract]
    public class LoadMyPlayerObjectOutput
    {
        [ProtoMember(1)]
        public DatabaseObject PlayerObject { get; set; }
    }
}