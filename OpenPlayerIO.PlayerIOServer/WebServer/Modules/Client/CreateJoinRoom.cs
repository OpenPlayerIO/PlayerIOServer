using System.Linq;
using Nancy;
using ProtoBuf;

namespace OpenPlayerIO.PlayerIOServer.WebServer.Modules.Client
{
    using Interfaces;
    using Helpers;

    using Messages.Client;
    using Messages.Construct;
    using Messages.Enums;
    using Messages.Error;

    public class CreateJoinRoom : NancyModule, IChannel
    {
        public int Channel { get; set; } = 27;

        public CreateJoinRoom()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post($"/api/{Channel}", delegate
            {
                var createJoinRoomArgs = Serializer.Deserialize<CreateJoinRoomArgs>(this.Request.Body);
                var createJoinRoomOutput = new CreateJoinRoomOutput();

                var playerToken = PlayerToken.Decode(this.Request.Headers["playertoken"].FirstOrDefault());
                switch (playerToken.State)
                {
                    case PlayerTokenState.Invalid: return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken is invalid." });
                    case PlayerTokenState.Expired: return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken has expired." });
                }

                createJoinRoomOutput.JoinKey = new JoinKey(playerToken.ConnectUserId, createJoinRoomArgs.ServerType, createJoinRoomArgs.RoomId).Encode();
                createJoinRoomOutput.Endpoints = new[] { new ServerEndpoint() { Address = "127.0.0.1", Port = 8184 } };

                return new ChannelResponse().Get(createJoinRoomOutput, this.Request.Headers["playertoken"].First());
            });
        }
    }
}