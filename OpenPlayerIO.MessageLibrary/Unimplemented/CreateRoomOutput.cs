using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class CreateRoomOutput
	{
		[ProtoMember(1)]
		public string RoomId { get; set; }
	}
}
