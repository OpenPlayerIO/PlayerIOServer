using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class SocialLoadProfilesArgs
	{
		[ProtoMember(1)]
		public string UserIds { get; set; }
	}
}
