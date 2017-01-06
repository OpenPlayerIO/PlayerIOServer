using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class NotificationsRefreshArgs
	{
		[ProtoMember(1)]
		public string LastVersion { get; set; }
	}
}
