using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class ListRoomsOutput
    {
        [ProtoMember(1)]
        public RoomInfo[] RoomInfo { get; set; }
    }
}