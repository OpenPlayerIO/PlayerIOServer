using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class SocialLoadProfilesOutput
	{
		[ProtoMember(1)]
		public Message Profiles { get; set; }
	}
}
