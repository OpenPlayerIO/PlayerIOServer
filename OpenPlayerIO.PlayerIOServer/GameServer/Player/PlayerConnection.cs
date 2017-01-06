using System;
using System.Linq;
using System.Net;
using SocketSlim.ChannelWrapper;
using OpenPlayerIO.PlayerIOServer.Helpers;

namespace OpenPlayerIO.PlayerIOServer.GameServer.Player
{
    using Enums;
    using Helpers;
    using Interfaces;

    internal class PlayerConnection : IConnection
    {
        internal BinarySerializer _serializer;
        internal BinaryDeserializer _deserializer;
        internal BasePlayer _player;
        internal ProtocolType _messageProtocol;
        internal ISocketChannel _channel;

        internal bool _connected = false;

        public PlayerConnection(BasePlayer player, ISocketChannel channel)
        {
            _serializer = new BinarySerializer();
            _deserializer = new BinaryDeserializer();

            _player = player;
            _channel = channel;
            _messageProtocol = ProtocolType.Auto;

            _player.IPAddress = IPAddress.Parse(_channel.RemoteEndPoint.ToString());

            _deserializer.OnDeserializedMessage += (message) => {
                if (!_connected) {
                    if (message.Type == "join") {
                        // find the game from the decrypted joinKey.
                        var joinKey = JoinKey.Decode(message.GetString(0));

                        if (joinKey != null) {
                            var game = _player.Host.Games.FirstOrDefault(g => g.RoomType == joinKey.ServerType);

                            if (game != null) {
                                _connected = true;

                                game.RoomId = joinKey.RoomId;

                                _player.ConnectUserId = joinKey.ConnectUserId;

                                _player.Game = game;
                                _player.Game.UserJoined(_player);

                                this.Send(new Message("playerio.joinresult", true));
                                return;
                            }
                        }

                        this.Send(new Message("playerio.joinresult", false));
                        _channel.Close();
                    }

                    return;
                }

                _player.Game.GotMessage(_player, message);
            };

            _channel.Closed += (s, e) => {
                _connected = false;

                if (_player.Game != null)
                    _player.Game.Disconnect(player);
            };

            _channel.BytesReceived += (s, bytes) => {
                switch (_messageProtocol) {
                    case ProtocolType.Auto:
                        switch (bytes[0]) {
                            case 0: _messageProtocol = ProtocolType.Binary; break;
                        }

                        if (bytes.Length > 1) {
                            _deserializer.AddBytes(bytes.Skip(1).ToArray());
                        }
                        break;

                    case ProtocolType.Binary:
                        _deserializer.AddBytes(bytes);
                        break;
                }
            };
        }

        /// <summary> Send a message to the connected client </summary>
        /// <param name="message"> The message to send </param>
        public void Send(Message message)
        {
            switch (_messageProtocol) {
                case ProtocolType.Auto:
                    throw new Exception("The player must be fully connected prior to sending a message.");
                case ProtocolType.Binary:
                    _channel.Send(_serializer.Serialize(message));
                    break;
            }
        }
    }
}