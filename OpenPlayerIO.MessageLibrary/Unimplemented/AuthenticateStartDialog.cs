using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class AuthenticateStartDialog
	{
		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public Message Arguments { get; set; }
	}
}
