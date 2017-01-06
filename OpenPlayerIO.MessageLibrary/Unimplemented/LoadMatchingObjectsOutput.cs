using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class LoadMatchingObjectsOutput
	{
		[ProtoMember(1)]
		public Message Objects { get; set; }
	}
}
