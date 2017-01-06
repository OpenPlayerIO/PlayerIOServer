using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class SimpleUserGetSecureLoginInfoOutput
	{
		[ProtoMember(1)]
		public byte[] PublicKey { get; set; }

		[ProtoMember(2)]
		public string Nonce { get; set; }
	}
}
