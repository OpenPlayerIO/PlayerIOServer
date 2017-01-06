using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class JoinRoomOutput
    {
        [ProtoMember(1)]
        public string JoinKey { get; set; }

        [ProtoMember(2)]
        public ServerEndpoint[] Endpoints { get; set; }
    }
}