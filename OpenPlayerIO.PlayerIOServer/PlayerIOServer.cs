using System.Threading;

namespace OpenPlayerIO.PlayerIOServer
{
    using GameServer;
    using WebServer;

    public class PlayerIOServer
    {
        public static WebServerHost WebServer;
        public static GameServerHost GameServer;

        public static void Start()
        {
            DatabaseHost.Connect("mongodb://localhost");

            WebServer = new WebServerHost();
            GameServer = new GameServerHost();

            WebServer.Start();
            GameServer.Start();

            Thread.Sleep(-1);
        }
    }
}