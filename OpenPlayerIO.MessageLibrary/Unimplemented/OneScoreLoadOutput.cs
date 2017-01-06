using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class OneScoreLoadOutput
	{
		[ProtoMember(1)]
		public Message OneScores { get; set; }
	}
}
