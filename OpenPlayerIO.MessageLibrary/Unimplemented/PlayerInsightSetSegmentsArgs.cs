using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PlayerInsightSetSegmentsArgs
	{
		[ProtoMember(1)]
		public string Segments { get; set; }
	}
}
