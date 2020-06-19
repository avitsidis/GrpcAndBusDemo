using NServiceBus;
using System;
using System.Threading.Tasks;
using TodoService.Services;

namespace TodoService.Client.EventsListener
{
    public class SomethingHappenedHandler : IHandleMessages<TodoAddedEvent>
    {
        public Task Handle(TodoAddedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"TodoAddedEvent intercepted with id {message.Id}");
            return Task.CompletedTask;
        }
    }
}
