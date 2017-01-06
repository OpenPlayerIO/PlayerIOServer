using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class SocialRefreshOutput
	{
		[ProtoMember(1)]
		public Message MyProfile { get; set; }

		[ProtoMember(2)]
		public Message Friends { get; set; }

		[ProtoMember(3)]
		public string Blocked { get; set; }
	}
}
