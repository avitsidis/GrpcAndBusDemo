using NServiceBus;
using System;
using System.Threading.Tasks;
using TodoService.NServiceBus;

namespace TodoService.Client.EventsListener
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfiguration = NServiceBusHelper.GetEndpointConfiguration("Client.EventsListener");
            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpointInstance.Stop();
        }
    }
}
