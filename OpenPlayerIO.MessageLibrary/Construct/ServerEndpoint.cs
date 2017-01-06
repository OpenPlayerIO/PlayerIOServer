using ProtoBuf;

namespace OpenPlayerIO.Messages.Construct
{
    [ProtoContract]
    public class ServerEndpoint
    {
        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public int Port { get; set; }
    }
}