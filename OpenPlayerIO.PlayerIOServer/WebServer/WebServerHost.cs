using System.IO;
using Microsoft.AspNetCore.Hosting;
using Nancy.Hosting.Kestrel;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace OpenPlayerIO.PlayerIOServer.WebServer
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
        }
    }

    public class WebServerHost
    {
        private IWebHost Server { get; set; }

        public WebServerHost()
        {
            Server = new WebHostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseKestrel()
            .UseStartup<Startup>()
            .UseUrls("http://localhost:80/")
            .Build();
        }

        public void Start() => Server.StartAsync();

        public void Stop() => Server.StopAsync();
    }
}