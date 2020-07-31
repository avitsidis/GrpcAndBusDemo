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
    public sealed class NServiceBus : IBus
    {
        private IMessageSession endpointInstance = null;
        public NServiceBus(IMessageSession endpointInstance)
        {
            this.endpointInstance = endpointInstance ?? throw new ArgumentNullException(nameof(endpointInstance));
        }

        public Task Publish(object o)
        {
            return endpointInstance.Publish(o);
        }

        public Task Send(object o)
        {
            return endpointInstance.Send(o);
        }
    }
}
