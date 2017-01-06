using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class TestNotificationReadSentArgs
	{
		[ProtoMember(1)]
		public string ConnectedUserId { get; set; }

		[ProtoMember(2)]
		public string GameId { get; set; }
	}
}
