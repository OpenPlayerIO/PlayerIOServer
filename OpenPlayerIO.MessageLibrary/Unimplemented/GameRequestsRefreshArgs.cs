using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class GameRequestsRefreshArgs
	{
		[ProtoMember(1)]
		public string PlayCodes { get; set; }
	}
}
