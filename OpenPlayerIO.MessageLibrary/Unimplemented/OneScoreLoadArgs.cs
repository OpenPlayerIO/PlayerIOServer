using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class OneScoreLoadArgs
	{
		[ProtoMember(1)]
		public string UserIds { get; set; }
	}
}
