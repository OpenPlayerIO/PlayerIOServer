using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class NotificationsSendArgs
	{
		[ProtoMember(1)]
		public Message Notifications { get; set; }
	}
}
