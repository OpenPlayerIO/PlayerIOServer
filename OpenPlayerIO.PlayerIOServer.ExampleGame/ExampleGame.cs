using System;

namespace OpenPlayerIO.PlayerIOServer.ExampleGame
{
    using GameServer;
    using GameServer.Attributes;
    using GameServer.Helpers;
    using GameServer.Player;

    public class MyPlayer : BasePlayer
    {
        public string Name;
    }

    [RoomType("MyRoom")]
    public class ExampleGame : Game<MyPlayer>
    {
        public override void GotMessage(MyPlayer player, Message message)
        {
            Console.WriteLine($"user {player.Id} sent {message}.");

            if (message.Type == "name")
                player.Name = message.GetString(0);
        }

        public override void GameStarted()
        {
            Console.WriteLine("game started");
        }

        public override void UserJoined(MyPlayer player)
        {
            Console.WriteLine($"user {player.Id} joined!");
        }

        public override void UserLeft(MyPlayer player)
        {
            Console.WriteLine($"user {player.Id} left!");
        }
    }
}
