using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PlayerInsightRefreshOutput
	{
		[ProtoMember(1)]
		public Message State { get; set; }
	}
}
