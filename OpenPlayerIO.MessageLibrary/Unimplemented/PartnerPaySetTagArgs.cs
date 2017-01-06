using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PartnerPaySetTagArgs
	{
		[ProtoMember(1)]
		public string PartnerId { get; set; }
	}
}
