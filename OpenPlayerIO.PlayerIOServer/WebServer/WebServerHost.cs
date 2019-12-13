using System;
using Nancy.Hosting.Kestrel;

namespace OpenPlayerIO.PlayerIOServer.WebServer
{
    public class WebServerHost
    {
        public NancyHost Server { get; set; }

        public WebServerHost()
        {
            this.Server = new NancyHost(new Uri($"http://localhost:80"));
        }

        public void Start() => Server.Start();

        public void Stop() => Server.Stop();
    }
}