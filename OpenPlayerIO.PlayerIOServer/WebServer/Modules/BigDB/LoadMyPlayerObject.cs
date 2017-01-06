using System.Linq;
using Nancy;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace OpenPlayerIO.PlayerIOServer.WebServer.Modules.BigDB
{
    using Interfaces;
    using Helpers;

    using Messages.BigDB;
    using Messages.Enums;
    using Messages.Error;

    public class LoadMyPlayerObject : NancyModule, IChannel
    {
        public int Channel { get; set; } = 103;

        public LoadMyPlayerObject()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post[$"/api/{Channel}"] = _ => {
                var loadMyPlayerObjectOutput = new LoadMyPlayerObjectOutput() { PlayerObject = new DatabaseObject() };

                // return an error if the specified PlayerToken is invalid
                var playerToken = PlayerToken.Decode(this.Request.Headers["playertoken"].FirstOrDefault());
                switch (playerToken.State) {
                    case PlayerTokenState.Invalid: return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken is invalid." });
                    case PlayerTokenState.Expired: return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken has expired." });
                }

                var collection = DatabaseHost.Client.GetDatabase(playerToken.GameId).GetCollection<BsonDocument>("PlayerObjects");
                var document = collection.Find(Builders<BsonDocument>.Filter.Eq("Key", playerToken.ConnectUserId)).FirstOrDefault();

                if (document == null)
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.BigDBObjectDoesNotExist });

                // retrieve the document version if available
                if (document.TryGetValue("_version", out var value))
                    loadMyPlayerObjectOutput.PlayerObject.Version = value.AsString;

                // remove any lingering artifacts before deserialization
                foreach (var element in document.Elements.ToArray())
                    if (element.Name.StartsWith("_"))
                        document.RemoveElement(element);

                var _databaseObject = JsonConvert.DeserializeObject<SentDatabaseObject>(document.ToJson());

                loadMyPlayerObjectOutput.PlayerObject.Table = _databaseObject.Table;
                loadMyPlayerObjectOutput.PlayerObject.Key = _databaseObject.Key;
                loadMyPlayerObjectOutput.PlayerObject.Properties = _databaseObject.Properties;

                return new ChannelResponse().Get(loadMyPlayerObjectOutput, this.Request.Headers["playertoken"].First());
            };
        }
    }
}