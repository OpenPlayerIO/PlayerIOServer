using System.Threading;

namespace OpenPlayerIO.PlayerIOServer
{
    using GameServer;
    using WebServer;

    public class PlayerIOServer
    {
        public static WebServerHost WebServer;
        public static GameServerHost GameServer;

        private const string _defaultMongoDBConnectionString = "mongodb://localhost";

        public static void Start(int httpPort = WebServerHost.defaultHttpPort, string mongoDBConnectionString = _defaultMongoDBConnectionString)
        {
            DatabaseHost.Connect(mongoDBConnectionString);

            WebServer = new WebServerHost(httpPort);
            GameServer = new GameServerHost();

            WebServer.Start();
            GameServer.Start();

            Thread.Sleep(-1);
        }
    }
}