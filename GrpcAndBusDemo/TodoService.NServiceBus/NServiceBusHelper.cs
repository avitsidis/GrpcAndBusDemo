using NServiceBus;
using NServiceBus.ProtoBufGoogle;
using System.Collections.Generic;

namespace TodoService.NServiceBus
{
    public class NServiceBusHelper
    {
        public static EndpointConfiguration GetEndpointConfiguration(string name, IEnumerable<NServicebusRoute> routes)
        {
            var endpointConfiguration = new EndpointConfiguration(name);
            endpointConfiguration.UseSerialization<ProtoBufGoogleSerializer>();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(type => type.Name.EndsWith("Event"));

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var routing = transport.Routing();
            foreach (var route in routes)
            {
                routing.RouteToEndpoint(route.Type, route.Destination);
            }

            return endpointConfiguration;
        }
        public static EndpointConfiguration GetEndpointConfiguration(string name)
        {
            var routes = new List<NServicebusRoute>();
            return GetEndpointConfiguration(name, routes);
        }

        public static EndpointConfiguration GetSendOnlyEndpointConfiguration(string name, IEnumerable<NServicebusRoute> routes)
        {
            var endpointConfiguration = GetEndpointConfiguration(name, routes);
            endpointConfiguration.SendOnly();
            return endpointConfiguration;
        }


    }
}
