using NServiceBus;
using NServiceBus.ProtoBufGoogle;
using System;
using System.Threading.Tasks;
using TodoService.Services;

namespace TodoService.Client.EventsListener
{
    class Program
    {
        private static EndpointConfiguration GetEndpointConfiguration()
        {
            var endpointConfiguration = new EndpointConfiguration("Client.EventsListener");
            endpointConfiguration.UseSerialization<ProtoBufGoogleSerializer>();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(type => type.Name.EndsWith("Event"));

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            

            return endpointConfiguration;
        }
        static async Task Main(string[] args)
        {
            var endpointConfiguration = GetEndpointConfiguration();

            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpointInstance.Stop();
        }
    }
}
