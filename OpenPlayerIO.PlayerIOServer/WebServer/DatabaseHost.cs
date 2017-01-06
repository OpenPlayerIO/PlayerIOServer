using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace OpenPlayerIO.PlayerIOServer.WebServer
{
    public static class DatabaseHost
    {
        public static MongoClient Client { get; set; }

        public static void Connect(string connectionString)
        {
            Client = new MongoClient(connectionString);
        }
    }
}
