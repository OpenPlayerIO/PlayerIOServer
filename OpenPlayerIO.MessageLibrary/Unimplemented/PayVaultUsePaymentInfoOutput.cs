using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
	[ProtoContract]
	public class PayVaultUsePaymentInfoOutput
	{
		[ProtoMember(1)]
		public Message ProviderResults { get; set; }

		[ProtoMember(2)]
		public Message VaultContents { get; set; }
	}
}
