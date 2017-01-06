using System;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using SocketSlim;

namespace OpenPlayerIO.PlayerIOServer.GameServer
{
    using Attributes;
    using Extensions;

    using Player;

    public class GameServerHost
    {
        public List<BaseGame> Games { get; set; }
        public ServerSocketSlim Server { get; set; }

        public GameServerHost()
        {
            // create a list of games to host
            this.Games = new List<BaseGame>();

            // create socket and set it up
            this.Server = new ServerSocketSlim(contiguous: true, sendReceiveBufferSize: 65536, maxSimultaneousConnections: 1000) {
                ListenAddress = IPAddress.Parse("::"),
                ListenPort = 8184
            };

            // when the server is started
            this.Server.StateChanged += (s, state) => {
                if (state.NewState == ServerState.Listening) {
                    foreach (var game in this.Games) {
                        var roomType = game.GetType().GetAttributeValue((RoomTypeAttribute rt) => rt.Type);

                        game.Setup(host: this, roomType: roomType);
                        game.GameStarted();
                    }
                }
            };

            // subscribe to relevant events
            this.Server.Connected += (s, args) => {
                var channel = args.Channel;
                var player = new BasePlayer() { Host = this };

                // the join key provided after connection will determine the requested game
                player.Connection = new PlayerConnection(player, channel);
            };

            // grab the instances of games found in the executing assembly
            var instances = from assembly in AppDomain.CurrentDomain.GetAssemblies() where assembly != Assembly.GetExecutingAssembly()
                            from type in assembly.GetTypes() where typeof(BaseGame).IsAssignableFrom(type) select type;

            // add found games to the list of games
            this.Games.AddRange(from game in instances select Activator.CreateInstance(game) as BaseGame);
        }

        public void Start() => Server.Start();

        public void Stop() => Server.Stop();
    }
}