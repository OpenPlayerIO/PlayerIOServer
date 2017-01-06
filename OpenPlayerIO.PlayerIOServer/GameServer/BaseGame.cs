namespace OpenPlayerIO.PlayerIOServer.GameServer
{
    using Helpers;

    using Player;

    public abstract class BaseGame
    {
        public abstract int PlayerCount { get; }
        public abstract string RoomId { get; set; }

        internal abstract string RoomType { get; set; }

        internal abstract void Setup(GameServerHost host, string roomType);

        internal abstract void GotMessage(BasePlayer player, Message message);

        internal abstract void Disconnect(BasePlayer player);

        internal abstract void UserJoined(BasePlayer player);

        internal abstract void UserLeft(BasePlayer player);

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