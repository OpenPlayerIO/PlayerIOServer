using System;
using System.Net;

namespace OpenPlayerIO.PlayerIOServer.GameServer.Player
{
    using Helpers;
    using Interfaces;

    [Serializable]
    public class BasePlayer
    {
        public IConnection Connection { get; internal set; }
        public IPAddress IPAddress { get; internal set; }

        public string ConnectUserId { get; internal set; }
        public int Id { get; internal set; }

        /// <summary> Send a message to the connected client </summary>
        /// <param name="message"> The message to send </param>
        public void Send(Message message)
        {
            this.Connection.Send(message);
        }

        /// <summary> Send a message to the connected client </summary>
        /// <param name="type"> The type of message to send </param>
        /// <param name="parameters"> The data to put in the message to send </param>
        public void Send(string type, params object[] parameters)
        {
            this.Connection.Send(new Message(type, parameters));
        }

        public void Disconnect()
        {
            this.Game.Disconnect(this);
        }

        internal BaseGame Game;
        internal GameServerHost Host;
    }
}