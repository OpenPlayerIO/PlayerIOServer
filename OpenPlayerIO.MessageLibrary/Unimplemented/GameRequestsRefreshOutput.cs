using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class GameRequestsRefreshOutput
	{
		[ProtoMember(1)]
		public Message Requests { get; set; }

		[ProtoMember(2)]
		public bool MoreRequestsWaiting { get; set; }

		[ProtoMember(3)]
		public string NewPlayCodes { get; set; }
	}
}
