using NServiceBus;
using System;
using System.Threading.Tasks;
using TodoService.Domain;
using TodoService.Services;

namespace TodoService.Hosting.MessageHandlers
{
    public class TodoCommandsHandler : IHandleMessages<AddCommand>
    {
        private readonly ITodoItemRepository repository;

        public TodoCommandsHandler(ITodoItemRepository repository)
        {
            this.repository = repository;
        }
        public async Task Handle(AddCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"command received with id {message.Id}");
            var newTodoItem = new TodoItem(Guid.Parse(message.Id), message.Title, message.DueDate.ToDateTime());
            repository.Add(newTodoItem);
            await context.Publish(new TodoAddedEvent
            {
                Id = newTodoItem.Id.ToString(),
                Title = newTodoItem.Title
            });
        }
    }
}
