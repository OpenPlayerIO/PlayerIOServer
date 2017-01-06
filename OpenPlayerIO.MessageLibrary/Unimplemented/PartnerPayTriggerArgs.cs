using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PartnerPayTriggerArgs
	{
		[ProtoMember(1)]
		public string Key { get; set; }

		[ProtoMember(2)]
		public uint Count { get; set; }
	}
}
