using System.Collections.Generic;
using System.Linq;

namespace OpenPlayerIO.PlayerIOServer.GameServer
{
    using Extensions;
    using Helpers;

    using Player;

    public abstract class Game<P> : BaseGame where P : BasePlayer, new()
    {
        internal GameServerHost Host { get; set; }
        internal int PlayerInstanceId { get; set; }

        public List<P> Players { get; set; }

        internal override string RoomType { get; set; }

        public override string RoomId { get; set; }
        public override int PlayerCount => this.Players.Count;

        internal override void Setup(GameServerHost host, string roomType)
        {
            this.Host = host;
            this.RoomType = roomType;

            this.Players = new List<P>();
            this.PlayerInstanceId = 0;
        }

        internal override void GotMessage(BasePlayer player, Message message)
        {
            var _player = this.Players.First(p => p.Id == player.Id);

            this.GotMessage(_player, message);
        }

        internal override void Disconnect(BasePlayer player)
        {
            var _player = this.Players.First(p => p.Id == player.Id);

            this.UserLeft(_player);
            this.Players.Remove(_player);
        }

        internal override void UserJoined(BasePlayer player)
        {
            player.Id = ++PlayerInstanceId;

            var _player = player.Cast<P>();

            this.Players.Add(_player);
            this.UserJoined(_player);
        }

        internal override void UserLeft(BasePlayer player)
        {
            var _player = this.Players.First(p => p.Id == player.Id);

            this.UserLeft(_player);
            this.Players.Remove(_player);
        }

        public virtual void GotMessage(P player, Message message)
        {
        }

        public virtual void UserJoined(P player)
        {
        }

        public virtual void UserLeft(P player)
        {
        }
    }
}