using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class AchievementsLoadArgs
	{
		[ProtoMember(1)]
		public string UserIds { get; set; }
	}
}
