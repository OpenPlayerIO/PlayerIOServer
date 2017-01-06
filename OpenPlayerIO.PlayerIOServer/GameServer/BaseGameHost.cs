using System;
using OpenPlayerIO.PlayerIOServer.GameServer.Helpers;

namespace OpenPlayerIO.PlayerIOServer.GameServer
{
    public abstract class BaseGameHost
    {
        public abstract void Broadcast(Message message, BasePlayer[] players, int playerCount);

        public Action<BasePlayer> UserJoined;
        public Action<BasePlayer> UserLeft;
    }
}