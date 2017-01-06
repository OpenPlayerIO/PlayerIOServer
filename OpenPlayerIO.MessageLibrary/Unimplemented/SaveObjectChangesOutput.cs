using ProtoBuf;

namespace OpenPlayerIO.Messages.Unimplemented
{
	[ProtoContract]
	public class SaveObjectChangesOutput
	{
		[ProtoMember(1)]
		public string Versions { get; set; }
	}
}
