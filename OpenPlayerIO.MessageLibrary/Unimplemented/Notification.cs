using PlayerIOClient.Helpers;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class Notification
	{
		[ProtoMember(1)]
		public string Recipient { get; set; }

		[ProtoMember(2)]
		public string EndpointType { get; set; }

		[ProtoMember(3)]
		public Message OldKeyValueData { get; set; }

		[ProtoMember(4)]
		public Message Data { get; set; }
	}
}
