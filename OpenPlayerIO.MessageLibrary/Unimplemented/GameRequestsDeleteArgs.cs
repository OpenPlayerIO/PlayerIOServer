using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class GameRequestsDeleteArgs
	{
		[ProtoMember(1)]
		public string RequestIds { get; set; }
	}
}
