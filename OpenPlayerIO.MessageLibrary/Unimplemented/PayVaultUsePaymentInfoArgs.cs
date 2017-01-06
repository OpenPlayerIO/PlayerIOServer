using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.PayVault
{
	[ProtoContract]
	public class PayVaultUsePaymentInfoArgs
	{
		[ProtoMember(1)]
		public string Provider { get; set; }

		[ProtoMember(2)]
		public Message ProviderArguments { get; set; }
	}
}
