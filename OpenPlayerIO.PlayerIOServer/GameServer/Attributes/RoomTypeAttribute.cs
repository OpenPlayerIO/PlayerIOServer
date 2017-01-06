using System;

namespace OpenPlayerIO.PlayerIOServer.GameServer.Attributes
{
    public class RoomTypeAttribute : Attribute
    {
        public RoomTypeAttribute(string type)
        {
            this.Type = type;
        }

        public readonly string Type;
    }
}