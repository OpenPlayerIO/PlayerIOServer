using ProtoBuf;

namespace OpenPlayerIO.Messages.Client
{
    [ProtoContract]
    public class KongregateConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string UserId { get; set; }

        [ProtoMember(3)]
        public string GameAuthToken { get; set; }
    }
}