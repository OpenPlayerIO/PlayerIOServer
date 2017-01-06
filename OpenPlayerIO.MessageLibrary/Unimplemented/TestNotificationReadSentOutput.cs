using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class TestNotificationReadSentOutput
	{
		[ProtoMember(1)]
		public Message TestNotifications { get; set; }
	}
}
