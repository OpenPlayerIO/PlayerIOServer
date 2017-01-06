using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class UsePayVaultTestInfoArgs
	{
		[ProtoMember(1)]
		public Message Info { get; set; }
	}
}
