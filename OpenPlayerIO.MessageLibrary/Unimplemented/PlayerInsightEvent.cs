using System;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class PlayerInsightEvent
	{
		[ProtoMember(1)]
		public string EventType { get; set; }

		[ProtoMember(2)]
		public Int32 Value { get; set; }
	}
}
