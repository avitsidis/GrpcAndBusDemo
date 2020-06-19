using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NServiceBus;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoService.Domain;
using TodoService.NServiceBus;

namespace TodoService.Services
{
    public class TodoGrpcService : TodoList.TodoListBase
    {
        //private readonly ILogger<TodoGrpcService> logger;
        private readonly ITodoItemRepository repository;
        private readonly IBus endpointInstance;

        public TodoGrpcService(/*ILogger<TodoGrpcService> logger,*/ ITodoItemRepository repository, IBus endpointInstance)
        {
            //this.logger = logger;
            this.repository = repository;
            this.endpointInstance = endpointInstance;
        }

        public override async Task<AddReply> Add(AddRequest request, ServerCallContext context)
        {
            var newTodoItem = new TodoItem(Guid.NewGuid(), request.Title, request.DueDate.ToDateTime());
            repository.Add(newTodoItem);
            await PublishTodoAddedEventFor(newTodoItem);
            return new AddReply
            {
                Item = AsMessage(newTodoItem)
            };
        }

        public override Task<GetAllReply> GetAll(GetAllRequest request, ServerCallContext context)
        {
            var reply = new GetAllReply();
            reply.Items.AddRange(repository.GetAll().Select(AsMessage));
            return Task.FromResult(reply);
        }
        private Task PublishTodoAddedEventFor(TodoItem newTodoItem)
        {
            return endpointInstance.Publish(new TodoAddedEvent
            {
                Id = newTodoItem.Id.ToString(),
                Title = newTodoItem.Title
            });
        }

        private TodoMessage AsMessage(TodoItem item)
        {
            return new TodoMessage
            { 
                Id = item.Id.ToString(),
                Title = item.Title,
                DueDate = Timestamp.FromDateTime(item.DueDate.ToUniversalTime()),
                IsCompleted = item.IsCompleted
            };
        }
    }
}
