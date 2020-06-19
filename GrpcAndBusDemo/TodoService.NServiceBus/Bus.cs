using NServiceBus;
using System;
using System.Threading.Tasks;

namespace TodoService.NServiceBus
{
    public interface IBus
    {
        Task Publish(object o);
        Task Send(object o);
    }

    //TODO review that part
    public sealed class Bus : IBus
    {
        public static Bus Instance { get; } =  new Bus();
        private static bool IsInitialized = false;
        private static IEndpointInstance EndpointInstance = null;
        public static void Initialize(IEndpointInstance endpointInstance)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("Cannot instatiate endpoint instance twice");
            }
            EndpointInstance = endpointInstance ?? throw new ArgumentNullException(nameof(endpointInstance));
            IsInitialized = true;
        }

        private Bus()
        {

        }

        public Task Publish(object o)
        {
            return EndpointInstance.Publish(o);
        }

        public Task Send(object o)
        {
            return EndpointInstance.Send(o);
        }
    }
}
