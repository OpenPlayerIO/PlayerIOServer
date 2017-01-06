using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class AchievementsRefreshArgs
	{
		[ProtoMember(1)]
		public string LastVersion { get; set; }
	}
}
