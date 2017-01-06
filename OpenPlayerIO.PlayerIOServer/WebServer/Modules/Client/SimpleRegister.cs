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
    using System.Text.RegularExpressions;

    public class SimpleRegister : NancyModule, IChannel
    {
        public int Channel { get; set; } = 403;

        public SimpleRegister()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post[$"/api/{Channel}"] = _ => {
                var simpleRegisterArgs = Serializer.Deserialize<SimpleRegisterArgs>(this.Request.Body);
                var simpleRegisterOutput = new SimpleRegisterOutput();

                var database = DatabaseHost.Client.GetDatabase(simpleRegisterArgs.GameId);
                var collection = database.GetCollection<UserAccount>("_accounts");

                // ensure the email address is valid (disregarding the TLD)
                var validEmailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                if (!Regex.IsMatch(simpleRegisterArgs.Email, validEmailRegex, RegexOptions.IgnoreCase)) {
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InvalidRegistrationData, Message = "The specified email address is invalid." });
                }

                if (collection.AsQueryable().Where(x => x.Name == simpleRegisterArgs.Username).Any()) {
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InvalidRegistrationData, Message = "The specified name is already in use." });
                }

                if (collection.AsQueryable().Where(x => x.Email == simpleRegisterArgs.Email).Any()) {
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InvalidRegistrationData, Message = "The specified email address is already in use." });
                }

                var registrationDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var registeredUserAccount = new UserAccount() {
                    Type = AccountType.Simple,
                    ConnectUserId = $"simple{registrationDate}x{PlayerIOEncrypt.CRNG.Next(0, 99)}",
                    Name = simpleRegisterArgs.Username,
                    Email = simpleRegisterArgs.Email,
                    Password = simpleRegisterArgs.Password,
                    Registered = registrationDate
                };

                collection.InsertOne(registeredUserAccount);

                simpleRegisterOutput.UserId = registeredUserAccount.ConnectUserId;
                simpleRegisterOutput.Token = new PlayerToken(simpleRegisterArgs.GameId, registeredUserAccount.ConnectUserId).Encode();

                return new ChannelResponse().Get(simpleRegisterOutput, simpleRegisterOutput.Token);
            };
        }
    }
}