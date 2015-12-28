using System;
using Nowin;
using Beginor.Owin.StaticFile;
using Owin;
using Microsoft.Owin.Builder;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace StaticFileMiddlewareDemo {

    public class MainClass {

        public static void Configure(IAppBuilder app) {
            var options = new StaticFileMiddlewareOptions {
                RootDirectory = "../wwwroot",
                DefaultFile = "index.html",
                EnableETag = true,
                EnableHtml5LocationMode = true
            };
            app.UseStaticFile(options);
        }

        public static void Main(string[] args) {
            // create a new AppBuilder
            IAppBuilder app = new AppBuilder();
            // init nowin's owin server factory.
            OwinServerFactory.Initialize(app.Properties);

            Configure(app);

            var serverBuilder = new ServerBuilder();
            const string ip = "127.0.0.1";
            const int port = 8888;
            serverBuilder.SetAddress(IPAddress.Parse(ip)).SetPort(port)
                .SetOwinApp(app.Build())
                .SetOwinCapabilities((IDictionary<string, object>)app.Properties[OwinKeys.ServerCapabilitiesKey]);

            using (var server = serverBuilder.Build()) {

                var serverRef = new WeakReference<INowinServer>(server);

                Task.Run(() => {
                    INowinServer nowinServer;
                    if (serverRef.TryGetTarget(out nowinServer)) {
                        nowinServer.Start();
                    }
                });

                var baseAddress = "http://" + ip + ":" + port + "/";
                Console.WriteLine("Nowin server listening {0}, press ENTER to exit.", baseAddress);

                Console.ReadLine();
            }
        }
    }
}
