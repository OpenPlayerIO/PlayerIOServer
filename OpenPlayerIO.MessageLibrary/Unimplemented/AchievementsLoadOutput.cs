using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class AchievementsLoadOutput
	{
		[ProtoMember(1)]
		public Message UserAchievements { get; set; }
	}
}
