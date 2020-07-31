using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SimpleInjector;
using TodoService.Data.InMemory;
using TodoService.Domain;
using TodoService.NServiceBus;
using TodoService.Services;

namespace TodoService.Hosting
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var container = CompositionRoot.Container;
            var aspNetHost = CreateHostBuilder(args).Build();
            //aspNetHost.UseSimpleInjector(CompositionRoot.Container);
            //aspNetHost.UseSimpleInjector(CompositionRoot.Container);
            await aspNetHost.StartAsync();
            //var endpointConfiguration = container.GetInstance<EndpointConfiguration>();
            //var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            //Bus.Initialize(endpointInstance);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await aspNetHost.StopAsync();
            //await endpointInstance.Stop();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(configAction => {
                    configAction.RegisterInstance<ITodoItemRepository>(new InMemoryTodoItemRepository());
                    configAction.RegisterType<TodoGrpcService>();
                    configAction.RegisterType<NServiceBus.NServiceBus>().As<IBus>();
                }))
                .UseMicrosoftLogFactoryLogging()
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = NServiceBusHelper.GetEndpointConfiguration("TodoService");
                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                ;
    }
}
