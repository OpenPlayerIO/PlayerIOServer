using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PlayerInsightTrackEventsArgs
	{
		[ProtoMember(1)]
		public Message Events { get; set; }
	}
}
