using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class OneScoreAddOutput
	{
		[ProtoMember(1)]
		public Message OneScore { get; set; }
	}
}
