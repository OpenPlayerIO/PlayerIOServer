using System;
using Nancy;
using ProtoBuf;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace OpenPlayerIO.PlayerIOServer.WebServer.Modules.Client
{
    using Helpers;
    using Database;

    using Messages.Client;
    using Messages.Enums;
    using Messages.Error;

    using Modules.Interfaces;

    public class SimpleConnect : NancyModule, IChannel
    {
        public int Channel { get; set; } = 400;

        public SimpleConnect()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post($"/api/{Channel}", delegate
            {
                var simpleConnectArgs = Serializer.Deserialize<SimpleConnectArgs>(this.Request.Body);
                var simpleConnectOutput = new SimpleConnectOutput();

                var database = DatabaseHost.Client.GetDatabase(simpleConnectArgs.GameId);
                var collection = database.GetCollection<UserAccount>("_accounts");

                // retrieve the account from the database or return null if none found.
                var account = collection.AsQueryable().Where(acc =>
                    (acc.Email == simpleConnectArgs.UsernameOrEmail || acc.ConnectUserId == simpleConnectArgs.UsernameOrEmail) && acc.Password == simpleConnectArgs.Password).FirstOrDefault();

                // ensure the proper authentication details have been provided
                if (account == null)
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnknownUser });

                simpleConnectOutput.UserId = account.ConnectUserId;
                simpleConnectOutput.Token = new PlayerToken(simpleConnectArgs.GameId, simpleConnectOutput.UserId, DateTimeOffset.UtcNow.AddHours(24)).Encode();

                return new ChannelResponse().Get(simpleConnectOutput, null, false);
            });
        }
    }
}