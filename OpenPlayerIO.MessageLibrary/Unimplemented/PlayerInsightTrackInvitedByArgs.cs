using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PlayerInsightTrackInvitedByArgs
	{
		[ProtoMember(1)]
		public string InvitingUserId { get; set; }

		[ProtoMember(2)]
		public string InvitationChannel { get; set; }
	}
}
