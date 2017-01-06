using System;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class OneScoreAddArgs
	{
		[ProtoMember(1)]
		public Int32 Score { get; set; }
	}
}
