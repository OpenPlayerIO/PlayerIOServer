using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenPlayerIO.PlayerIOServer.GameServer.Helpers
{
    /// <summary>
    /// Represents a message sent between the client and the server. 
    /// <para> A message consists of a type (string), and zero or more parameters that are supported. </para>
    /// </summary>
    public class Message : IEnumerable<object>
    {
        #region Properties
        public string Type { get; private set; }
        private List<Tuple<object, ObjectType>> Values { get; set; }
        public int Count => this.Values.Count;
        #endregion

        #region Constructors
        public static Message Create(string type, params object[] parameters) => new Message(type, parameters);
        public Message(string type, params object[] parameters)
        {
            this.Type = type;
            this.Values = new List<Tuple<object, ObjectType>>();

            this.AddValues(parameters);
        }
        #endregion

        #region Enumerators
        public IEnumerator<object> GetEnumerator() => this.Values.Select(t => t.Item1).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region Accessors
        public object this[uint index] => this.Values[(int)index].Item1;
        #endregion

        #region Methods

        #region Get
        public int    GetInteger(uint index)   => (int)this[index];
        public uint   GetUInteger(uint index)  => (uint)this[index];
        public long   GetLong(uint index)      => (long)this[index];
        public ulong  GetULong(uint index)     => (ulong)this[index];
        public double GetDouble(uint index)    => (double)this[index];
        public float  GetFloat(uint index)     => (float)this[index];
        public string GetString(uint index)    => (string)this[index];
        public byte[] GetByteArray(uint index) => (byte[])this[index];
        public bool   GetBoolean(uint index)   => (bool)this[index];
        #endregion

        #region Add
        public Message Add(int parameter)    => AddValues(parameter);
        public Message Add(uint parameter)   => AddValues(parameter);
        public Message Add(long parameter)   => AddValues(parameter);
        public Message Add(ulong parameter)  => AddValues(parameter);
        public Message Add(double parameter) => AddValues(parameter);
        public Message Add(float parameter)  => AddValues(parameter);
        public Message Add(string parameter) => AddValues(parameter);
        public Message Add(byte[] parameter) => AddValues(parameter);
        public Message Add(bool parameter)   => AddValues(parameter);
        public Message Add(object parameter) => AddValues(parameter);
        #endregion

        #region Private/Internal
        private Message AddValues(params object[] values) {
            foreach (var value in values) {
                var type = ObjectType.Unknown;

                switch (value) {
                    case int    _: type = ObjectType.Integer;   break;
                    case uint   _: type = ObjectType.UInteger;  break;
                    case long   _: type = ObjectType.Long;      break;
                    case ulong  _: type = ObjectType.ULong;     break;
                    case double _: type = ObjectType.Double;    break;
                    case float  _: type = ObjectType.Float;     break;
                    case string _: type = ObjectType.String;    break;
                    case byte[] _: type = ObjectType.ByteArray; break;
                    case bool   _: type = ObjectType.Boolean;   break;
                    case object _:
                        if (value is null)
                            throw new Exception("You cannot add null values to Player.IO Messages.");
                        break;
                    default: throw new InvalidOperationException(string.Format(string.Concat($"Player.IO Messages only support objects of types: {0} Type '{value.GetType().FullName}' is not supported."),
                                                                 string.Join(", ", Enum.GetNames(typeof(ObjectType)))) + Environment.NewLine);
                }

                this.Values.Add(new Tuple<object, ObjectType>(value, type));
            }

            return this;
        }

        /// <summary> A string representation of the Message. </summary>
        public override string ToString()
        {
            var sb = new StringBuilder("");

            sb.AppendLine(string.Concat("  msg.Type= ", this.Type, ", ", this.Values.Count, " entries"));

            for (var i = 0; i < this.Values.Count; i++)
                sb.AppendLine(string.Concat("  msg[", i, "] = ", this.Values[i].Item1, "  (", this.Values[i].Item2, ")"));

            return sb.ToString();
        }
        #endregion

        #endregion

        #region Enums
        private enum ObjectType : byte
        {
            Unknown = 255,
            Integer = 0,
            UInteger = 1,
            Long = 2,
            ULong = 3,
            Double = 4,
            Float = 5,
            String = 6,
            ByteArray = 7,
            Boolean = 8
        }
        #endregion
    }
}