using System;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class OneScoreSetArgs
	{
		[ProtoMember(1)]
		public Int32 Score { get; set; }
	}
}
