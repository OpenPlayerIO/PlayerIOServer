using System.Linq;
using System.Collections.Generic;
using Nancy;
using ProtoBuf;
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

    public class LoadObjects : NancyModule, IChannel
    {
        public int Channel { get; set; } = 85;

        public LoadObjects()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post($"/api/{Channel}", delegate
            {
                var playerToken = PlayerToken.Decode(this.Request.Headers["playertoken"].FirstOrDefault());
                var loadObjectsArgs = Serializer.Deserialize<LoadObjectsArgs>(this.Request.Body);
                var loadObjectsOutput = new LoadObjectsOutput();

                switch (playerToken.State)
                {
                    case PlayerTokenState.Invalid:
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken is invalid." });
                    case PlayerTokenState.Expired:
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken has expired." });
                }

                // prevent loading an internal table from the BigDB database
                if (!loadObjectsArgs.ObjectIds.Table.All(char.IsLetterOrDigit))
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "Table names cannot contain non-alphanumeric characters." });

                // collection names may only contain no more than 64 characters and no less than 1 character
                if (loadObjectsArgs.ObjectIds.Table.Length < 1 || loadObjectsArgs.ObjectIds.Table.Length > 64)
                    return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "Table names must be between 1-64 characters in length." });

                // retrieve the relevant database objects from the BigDB database
                var collection = DatabaseHost.Client.GetDatabase(playerToken.GameId).GetCollection<BsonDocument>(loadObjectsArgs.ObjectIds.Table);
                var databaseObjects = new List<DatabaseObject>();

                foreach (var objectKey in loadObjectsArgs.ObjectIds.Keys)
                {
                    // prevent creating objects with invalid characters within the keys
                    if (!objectKey.All(char.IsLetterOrDigit))
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "DatabaseObject keys cannot contain non-alphanumeric characters." });

                    // database object keys may only contain no more than 64 characters and no less than 1 character
                    if (objectKey.Length < 1 || objectKey.Length > 64)
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "DatabaseObject keys must be between 1-64 characters in length." });

                    // query the database for keys matching the key specified
                    var document = collection.Find(Builders<BsonDocument>.Filter.Eq("Key", objectKey)).FirstOrDefault();

                    if (document != null)
                    {
                        var databaseObject = new DatabaseObject();

                        // retrieve the document version if available
                        if (document.TryGetValue("_version", out var value))
                            databaseObject.Version = value.AsString;

                        // remove any lingering artifacts before deserialization
                        foreach (var element in document.Elements.ToArray())
                            if (element.Name.StartsWith("_"))
                                document.RemoveElement(element);

                        var _databaseObject = JsonConvert.DeserializeObject<SentDatabaseObject>(document.ToJson());

                        databaseObject.Table = _databaseObject.Table;
                        databaseObject.Key = _databaseObject.Key;
                        databaseObject.Properties = _databaseObject.Properties;

                        databaseObjects.Add(databaseObject);
                    }
                }

                loadObjectsOutput.Objects = databaseObjects.ToArray();

                return new ChannelResponse().Get(loadObjectsOutput, this.Request.Headers["playertoken"].First());
            });
        }
    }
}