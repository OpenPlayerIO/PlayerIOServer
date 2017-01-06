using System.Linq;
using System.Collections.Generic;
using Nancy;
using ProtoBuf;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace OpenPlayerIO.PlayerIOServer.WebServer.Modules.BigDB
{
    using Interfaces;

    using Messages.BigDB;
    using Messages.Enums;
    using Messages.Error;

    using OpenPlayerIO.PlayerIOServer.Helpers;

    public class CreateObjects : NancyModule, IChannel
    {
        public int Channel { get; set; } = 82;

        public CreateObjects()
        {
            var response = new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnsupportedMethod });

            this.Post[$"/api/{Channel}"] = _ => {
                var createObjectsArgs = Serializer.Deserialize<CreateObjectsArgs>(this.Request.Body);
                var createObjectsOutput = new CreateObjectsOutput();

                var playerToken = PlayerToken.Decode(this.Request.Headers["playertoken"].FirstOrDefault());
                switch (playerToken.State) {
                    case PlayerTokenState.Invalid: return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken is invalid." });
                    case PlayerTokenState.Expired: return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.InternalError, Message = "The specified PlayerToken has expired." });
                }

                // if they request no objects, return no objects
                if (createObjectsArgs.Objects.Length < 1) {
                    createObjectsOutput.Objects = new DatabaseObject[0];

                    return new ChannelResponse().Get(createObjectsOutput, this.Request.Headers["playertoken"].First());
                }

                // To-Do: check for permissions to create objects
                var database = DatabaseHost.Client.GetDatabase(playerToken.GameId);
                var collection = database.GetCollection<BsonDocument>(createObjectsArgs.Objects[0].Table);

                var databaseObjects = new List<DatabaseObject>();
                foreach (var databaseObject in createObjectsArgs.Objects.OrderByDescending(x => x.Table)) {
                    // load collection once per unique table for performance reasons
                    if (databaseObject.Table != collection.CollectionNamespace.CollectionName)
                        collection = database.GetCollection<BsonDocument>(databaseObject.Table);

                    // prevent creating objects in an internal table from the BigDB database
                    if (!databaseObject.Table.All(char.IsLetterOrDigit))
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "Table names cannot contain non-alphanumeric characters." });

                    // prevent creating objects with invalid characters within the keys
                    if (!databaseObject.Key.All(char.IsLetterOrDigit))
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "DatabaseObject keys cannot contain non-alphanumeric characters." });

                    // table names may only contain no more than 64 characters and no less than 1 character
                    if (databaseObject.Table.Length < 1 || databaseObject.Table.Length > 64)
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "Table names must be between 1-64 characters in length." });

                    // database object keys may only contain no more than 64 characters and no less than 1 character
                    if (databaseObject.Key.Length < 1 || databaseObject.Key.Length > 64)
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.AccessDenied, Message = "DatabaseObject keys must be between 1-64 characters in length." });

                    // check if the collection exists in the respecctful database
                    if (!database.ListCollections(new ListCollectionsOptions() { Filter = new BsonDocument("name", databaseObject.Table) }).Any())
                        return new ChannelResponse().Get(new Error() { ErrorCode = (int)ErrorCode.UnknownTable });

                    var document = collection.Find(Builders<BsonDocument>.Filter.Eq("Key", playerToken.ConnectUserId)).FirstOrDefault();

                    // load existing database objects if required
                    if (createObjectsArgs.LoadExisting && document != null) {
                        var existingDatabaseObject = new DatabaseObject();

                        if (document.TryGetValue("_version", out var value))
                            existingDatabaseObject.Version = value.AsString;

                        // remove any lingering artifacts before deserialization
                        foreach (var element in document.Elements.ToArray())
                            if (element.Name.StartsWith("_"))
                                document.RemoveElement(element);

                        var _existingDatabaseObject = JsonConvert.DeserializeObject<SentDatabaseObject>(document.ToJson());
                        existingDatabaseObject.Table = _existingDatabaseObject.Table;
                        existingDatabaseObject.Key = _existingDatabaseObject.Key;
                        existingDatabaseObject.Properties = _existingDatabaseObject.Properties;

                        databaseObjects.Add(existingDatabaseObject);

                        // skip inserting the existing database object into the database
                        continue;
                    }

                    // insert the new database object into the database
                    collection.InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(databaseObject)).Add("_version", "1"));

                    // insert the database object into the the output
                    databaseObjects.Add(new DatabaseObject() {
                        Table = databaseObject.Table,
                        Key = databaseObject.Key,
                        Properties = databaseObject.Properties,
                        Version = "1"
                    });
                }

                createObjectsOutput.Objects = databaseObjects.ToArray();

                return new ChannelResponse().Get(createObjectsOutput, this.Request.Headers["playertoken"].First(), false);
            };
        }
    }
}
