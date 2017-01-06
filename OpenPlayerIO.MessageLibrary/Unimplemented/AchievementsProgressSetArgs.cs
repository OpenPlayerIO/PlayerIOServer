using System;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class AchievementsProgressSetArgs
	{
		[ProtoMember(1)]
		public string AchievementId { get; set; }

		[ProtoMember(2)]
		public Int32 Progress { get; set; }
	}
}
