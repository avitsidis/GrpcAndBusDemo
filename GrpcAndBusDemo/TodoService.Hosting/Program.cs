using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using TodoService.NServiceBus;

namespace TodoService.Hosting
{
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            var container = CompositionRoot.Container;
            
            var aspNetHost = CreateHostBuilder(args).Build();
            await aspNetHost.StartAsync();
            var endpointConfiguration = container.GetInstance<EndpointConfiguration>();
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            Bus.Initialize(endpointInstance);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await aspNetHost.StopAsync();
            await endpointInstance.Stop();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
