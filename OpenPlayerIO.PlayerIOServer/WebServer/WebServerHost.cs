using System.IO;
using Microsoft.AspNetCore.Hosting;
using Nancy.Hosting.Kestrel;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy.Bootstrapper;
using Nancy;
using Nancy.Configuration;

namespace OpenPlayerIO.PlayerIOServer.WebServer
{
    public class PlayerIONancyBootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(Nancy.Configuration.INancyEnvironment environment)
        {
#if DEBUG

            environment.Tracing(true, true);

#endif
        }
    }

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(
                x => x.UseNancy(
                    y => y.Bootstrapper = new PlayerIONancyBootstrapper()));
        }
    }

    public class WebServerHost
    {
        internal const int defaultHttpPort = 80;

        private IWebHost Server { get; set; }

        private void BuildWebServer(int port)
        {
            Server = new WebHostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseKestrel(option =>
            {
                //In order to keep current implementation with Kestrel working
                option.AllowSynchronousIO = true;
            })
            .UseStartup<Startup>()
            .UseUrls($"http://localhost:{port}/")
            .Build();
        }

        public WebServerHost()
        {
            BuildWebServer(defaultHttpPort);
        }

        public WebServerHost(int port)
        {
            BuildWebServer(port);
        }

        public void Start() => Server.StartAsync();

        public void Stop() => Server.StopAsync();
    }
}