using System.Net;
using OpenPlayerIO.PlayerIOServer.GameServer.Helpers;
using OpenPlayerIO.PlayerIOServer.GameServer.Interfaces;
using SocketSlim.ChannelWrapper;

namespace OpenPlayerIO.PlayerIOServer.GameServer
{
    public class BasePlayer
    {
        public IConnection Connection { get; internal set; }
        public IPAddress IPAddress { get; internal set; }

        public string ConnectUserId { get; internal set; }

        /// <summary> Send a message to the connected client </summary>
        /// <param name="message"> The message to send </param>
        public void Send(Message message)
        {
            this.Channel.Send(new BinarySerializer().Serialize(message));
        }

        /// <summary>
        /// Send a message to the connected client without first having to construct a Message object.
        /// </summary>
        /// <param name="type"> The type of message to send </param>
        /// <param name="parameters"> The data to put in the message to send </param>
        public void Send(string type, params object[] parameters)
        {
            this.Connection.Send(new Message(type, parameters));
        }

        internal BaseGameHost Host { get; set; }
        internal ISocketChannel Channel { get; set; }
        internal BinaryDeserializer Deserializer { get; set; }
    }
}