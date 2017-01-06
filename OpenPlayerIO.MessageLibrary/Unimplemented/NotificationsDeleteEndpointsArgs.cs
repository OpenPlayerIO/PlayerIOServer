using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class NotificationsDeleteEndpointsArgs
	{
		[ProtoMember(1)]
		public string LastVersion { get; set; }

		[ProtoMember(2)]
		public Message Endpoints { get; set; }
	}
}
