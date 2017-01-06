using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    using Messages.Enums;

    public partial class BigDBObjectValue
    {
        public object Value { get; set; }

        public BigDBObjectValue()
        {
        }

        internal BigDBObjectValue(ObjectType type, object value)
        {
            this.Type = type;
            this.Value = value;

            this.GetProperty()?.SetValue(this, this.Value);
            this.SetProperties();
        }

        public static BigDBObjectValue Create(object value)
        {
            switch (value) {
                case string _: return new BigDBObjectValue(ObjectType.String, value);
                case int _:    return new BigDBObjectValue(ObjectType.Int, value);
                case uint _:   return new BigDBObjectValue(ObjectType.UInt, value);
                case long _:   return new BigDBObjectValue(ObjectType.Long, value);
                case float _:  return new BigDBObjectValue(ObjectType.Float, value);
                case double _: return new BigDBObjectValue(ObjectType.Double, value);
                case bool _:   return new BigDBObjectValue(ObjectType.Bool, value);
                case byte[] _: return new BigDBObjectValue(ObjectType.ByteArray, value);

                case DateTime DateTime:              return new BigDBObjectValue(ObjectType.DateTime, DateTime.ToUnixTime());
                case DatabaseObject DatabaseObject:  return new BigDBObjectValue(ObjectType.DatabaseObject, DatabaseObject.Properties.ToArray());
                case DatabaseObject[] DatabaseArray: return new BigDBObjectValue(ObjectType.DatabaseArray, DatabaseArray.SelectMany(p => p.Properties).ToArray());

                default: throw new ArgumentException($"The type { value.GetType().FullName } is not supported.", "value");
            }
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (this.Value == null)
                this.Value = this.GetProperty()?.GetValue(this);

            this.SetProperties();
        }

        public PropertyInfo GetProperty()
        {
            foreach (var property in this.GetType().GetProperties()) {
                var attributes = property.GetCustomAttributes(typeof(ProtoMemberAttribute), true).Select(attribute => (ProtoMemberAttribute)attribute);

                foreach (var attribute in attributes)
                    if (attribute.Tag == (int)this.Type + 2)
                        return property;
            }

            return null;
        }

        public void SetProperties()
        {
            foreach (var property in this.GetType().GetProperties()) {
                var attributes = property.GetCustomAttributes(typeof(ProtoMemberAttribute), true).Select(attribute => (ProtoMemberAttribute)attribute)
                                 .Where(attribute => attribute.Tag == (int)this.Type + 2);

                foreach (var attribute in attributes) {
                    switch (this.Type) {
                        case ObjectType.DatabaseArray:
                        case ObjectType.DatabaseObject:
                            KeyValuePair<string, BigDBObjectValue>[] databaseObject;

                            if (this.Value is JArray) {
                                databaseObject = ((JArray)this.Value).ToObject<KeyValuePair<string, BigDBObjectValue>[]>();
                            } else {
                                databaseObject = (KeyValuePair<string, BigDBObjectValue>[])this.Value;
                            }

                            foreach (var keyValuePair in databaseObject)
                                keyValuePair.Value.SetProperties();

                            property.SetValue(this, databaseObject);
                            break;
                        default:
                            property.SetValue(this, Convert.ChangeType(this.Value, property.PropertyType));
                            break;
                    }
                }
            }
        }
    }
}