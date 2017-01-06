using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class CreateJoinRoomOutput
    {
        [ProtoMember(1)]
        public string RoomId { get; set; }

        [ProtoMember(2)]
        public string JoinKey { get; set; }

        [ProtoMember(3)]
        public ServerEndpoint[] Endpoints { get; set; }
    }
}