using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class CreateJoinRoomArgs
    {
        [ProtoMember(1)]
        public string RoomId { get; set; }

        [ProtoMember(2)]
        public string ServerType { get; set; }

        [ProtoMember(3)]
        public bool Visible { get; set; }

        [ProtoMember(4)]
        public KeyValuePair[] RoomData { get; set; }

        [ProtoMember(5)]
        public KeyValuePair[] JoinData { get; set; }

        [ProtoMember(6)]
        public bool IsDevRoom { get; set; }
    }
}