﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenPlayerIO.Messages.Enums;
using ProtoBuf;

namespace OpenPlayerIO.Messages.BigDB
{
    public partial class DatabaseArray : DatabaseObject
    {

    }

    public partial class DatabaseObject : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        public string Table { get; set; }

        public DatabaseObject()
        {
            this.Properties = new Dictionary<string, BigDBObjectValue>();
        }

        #region Enumerator
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var current in this.Properties)
                yield return new KeyValuePair<string, object>(current.Key, current.Value.Value);
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Properties.Select(x => x.Value).GetEnumerator();
        }
        #endregion Enumerator

        #region Methods
        public override string ToString() => JsonConvert.SerializeObject(this.Properties, Formatting.Indented);

        public bool Contains(string propertyExpression) => this.Properties.Any(property => property.Key == propertyExpression);

        public static DatabaseObject LoadFromJSON(string input) => JsonConvert.DeserializeObject<DatabaseObject>(input);

        public static DatabaseObject LoadFromProto(byte[] input) => Serializer.Deserialize<DatabaseObject>(new MemoryStream(input));

        #endregion

        #region Get
        public object this[string propertyExpression] => this.Properties.Where(x => x.Key == propertyExpression).FirstOrDefault().Value.Value;

        internal object this[string propertyExpression, ObjectType expectedType] => this.Properties.Where(x => x.Key == propertyExpression).FirstOrDefault().Value ?? throw new Exception("Invalid Type");

        public bool GetBool(string propertyExpression) => (bool)this[propertyExpression, ObjectType.Bool];

        public bool GetBool(string propertyExpression, bool defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (bool)this[propertyExpression, ObjectType.Bool] : defaultValue;

        public byte[] GetBytes(string propertyExpression) => (byte[])this[propertyExpression, ObjectType.ByteArray];

        public byte[] GetBytes(string propertyExpression, byte[] defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (byte[])this[propertyExpression, ObjectType.ByteArray] : defaultValue;

        public double GetDouble(string propertyExpression) => (double)this[propertyExpression, ObjectType.Double];

        public double GetDouble(string propertyExpression, double defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (double)this[propertyExpression, ObjectType.Double] : defaultValue;

        public float GetFloat(string propertyExpression) => (float)this[propertyExpression, ObjectType.Float];

        public float GetFloat(string propertyExpression, float defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (float)this[propertyExpression, ObjectType.Float] : defaultValue;

        public int GetInt(string propertyExpression) => (int)this[propertyExpression, ObjectType.Int];

        public int GetInt(string propertyExpression, int defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (int)this[propertyExpression, ObjectType.Int] : defaultValue;

        public long GetLong(string propertyExpression) => (long)this[propertyExpression, ObjectType.Long];

        public long GetLong(string propertyExpression, long defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (long)this[propertyExpression, ObjectType.Long] : defaultValue;

        public string GetString(string propertyExpression) => (string)this[propertyExpression, ObjectType.String];

        public string GetString(string propertyExpression, string defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (string)this[propertyExpression, ObjectType.String] : defaultValue;

        public uint GetUInt(string propertyExpression) => (uint)this[propertyExpression, ObjectType.UInt];

        public uint GetUInt(string propertyExpression, uint defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? (uint)this[propertyExpression, ObjectType.UInt] : defaultValue;

        public object GetValue(string propertyExpression) => this[propertyExpression];

        public object GetValue(string propertyExpression, object defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? this[propertyExpression] : defaultValue;

        public DateTime GetDateTime(string propertyExpression) => (((long)this[propertyExpression]).FromUnixTime());

        public DateTime GetDateTime(string propertyExpression, DateTime defaultValue) =>
                    this.Properties.Any(property => property.Key == propertyExpression) ? ((long)this[propertyExpression, ObjectType.DateTime]).FromUnixTime() : defaultValue;

        public DatabaseArray GetArray(string propertyExpression) => (DatabaseArray)this[propertyExpression, ObjectType.DatabaseArray];

        public DatabaseObject GetObject(string propertyExpression) => (DatabaseObject)this[propertyExpression, ObjectType.DatabaseObject];
        #endregion

        #region Set
        public virtual DatabaseObject Set(string propertyExpression, object value)
        {
            this.Properties.Add(propertyExpression, BigDBObjectValue.Create(value));

            return this;
        }
        #endregion

        #region Remove
        public void Remove(string propertyExpression)
        {
            this.Properties.Remove(propertyExpression);
        }

        public void Clear()
        {
            this.Properties.Clear();
        }
        #endregion
    }

    public static class Helpers
    {
        public static long ToUnixTime(this DateTime input) => (long)((input - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        public static DateTime FromUnixTime(this long input) => new DateTime(1970, 1, 1).AddMilliseconds((long)input);
    }
}