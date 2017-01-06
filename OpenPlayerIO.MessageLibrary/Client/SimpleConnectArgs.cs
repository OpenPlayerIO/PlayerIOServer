using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class SimpleConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string UsernameOrEmail { get; set; }

        [ProtoMember(3)]
        public string Password { get; set; }
    }
}