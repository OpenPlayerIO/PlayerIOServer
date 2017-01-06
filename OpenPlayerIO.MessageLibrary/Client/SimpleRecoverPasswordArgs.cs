using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class SimpleRecoverPasswordArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string UsernameOrEmail { get; set; }
    }
}