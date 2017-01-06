using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class DeleteIndexRangeArgs
	{
		[ProtoMember(1)]
		public string Table { get; set; }

		[ProtoMember(2)]
		public string Index { get; set; }

		[ProtoMember(3)]
		public Message StartIndexValue { get; set; }

		[ProtoMember(4)]
		public Message StopIndexValue { get; set; }
	}
}
