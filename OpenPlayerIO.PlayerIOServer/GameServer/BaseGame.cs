namespace OpenPlayerIO.PlayerIOServer.GameServer
{
    using Helpers;

    public abstract class BaseGame
    {
        public abstract int PlayerCount { get; }
        public abstract string RoomId { get; set; }

        internal abstract string RoomType { get; set; }

        internal abstract void Setup(GameServerHost host, string roomType);

        internal abstract void GotMessage(Player.BasePlayer player, Message message);

        internal abstract void Disconnect(Player.BasePlayer player);

        internal abstract void UserJoined(Player.BasePlayer player);

        internal abstract void UserLeft(Player.BasePlayer player);

        /// <summary> This method is called when a game is closed. </summary>
        public virtual void GameClosed()
        {
        }

        /// <summary> This method is called when a game is created. </summary>
        public virtual void GameStarted()
        {
        }
    }
}