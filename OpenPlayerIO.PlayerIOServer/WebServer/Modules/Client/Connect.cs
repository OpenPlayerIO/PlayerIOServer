using System;
using Nancy;
using ProtoBuf;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace OpenPlayerIO.PlayerIOServer.WebServer.Modules.Client
{
    using Messages.Client;
    using Messages.Enums;
    using Messages.Error;

    using Database;
    using Helpers;

    using Modules.Interfaces;

    public class Connect : NancyModule, IChannel
    {
        public int Channel { get; set; } = 10;

        public Connect()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post($"/api/{Channel}", delegate
            {
                var connectArgs = Serializer.Deserialize<ConnectArgs>(this.Request.Body);
                var connectOutput = new ConnectOutput();

                var database = DatabaseHost.Client.GetDatabase(connectArgs.GameId);
                var collection = database.GetCollection<UserAccount>("_accounts");

                // ensure the proper authentication details have been provided
                if (collection.AsQueryable().Where(account => account.ConnectUserId == connectArgs.UserId && account.Password == connectArgs.Auth).Any())
                {
                    connectOutput.UserId = connectArgs.UserId;
                    connectOutput.Token = new PlayerToken(connectArgs.GameId, connectOutput.UserId, DateTimeOffset.UtcNow.AddHours(24)).Encode();

                    return new ChannelResponse().Get(connectOutput, connectOutput.Token);
                }

                return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InvalidAuth });
            });
        }
    }
}