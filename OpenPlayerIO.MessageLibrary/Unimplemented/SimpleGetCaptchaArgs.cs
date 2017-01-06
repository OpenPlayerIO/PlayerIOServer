using System;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class SimpleGetCaptchaArgs
	{
		[ProtoMember(1)]
		public string GameId { get; set; }

		[ProtoMember(2)]
		public Int32 Width { get; set; }

		[ProtoMember(3)]
		public Int32 Height { get; set; }
	}
}
