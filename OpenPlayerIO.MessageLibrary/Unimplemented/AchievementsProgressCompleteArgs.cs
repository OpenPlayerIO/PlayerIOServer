using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class AchievementsProgressCompleteArgs
	{
		[ProtoMember(1)]
		public string AchievementId { get; set; }
	}
}
