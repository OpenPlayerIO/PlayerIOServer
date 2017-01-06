using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class OneScoreSetOutput
	{
		[ProtoMember(1)]
		public Message OneScore { get; set; }
	}
}
