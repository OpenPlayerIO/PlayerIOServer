using System.Collections.Generic;
using OpenPlayerIO.Messages.Construct;
using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
	public class PlayerInsightState
	{
		[ProtoMember(1)]
		public int PlayersOnline { get; set; }

		[ProtoMember(2)]
		public KeyValuePair[] Segments { get; set; }
	}
}
