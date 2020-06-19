using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NServiceBus;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoService.Domain;

namespace TodoService.Services
{
    //TODO should be splitted in 2 classes
    public class TodoGrpcService : TodoList.TodoListBase, IHandleMessages<AddCommand>
    {
        //private readonly ILogger<TodoGrpcService> logger;
        private readonly ITodoItemRepository repository;

        public TodoGrpcService(/*ILogger<TodoGrpcService> logger,*/ ITodoItemRepository repository)
        {
            //this.logger = logger;
            this.repository = repository;
        }

        public override Task<AddReply> Add(AddRequest request, ServerCallContext context)
        {
            var newTodoItem = new TodoItem(Guid.NewGuid(), request.Title, request.DueDate.ToDateTime());
            repository.Add(newTodoItem);
            return Task.FromResult(new AddReply
            {
                Item = AsMessage(newTodoItem)
            }) ;
        }

        public override Task<GetAllReply> GetAll(GetAllRequest request, ServerCallContext context)
        {
            var reply = new GetAllReply();
            reply.Items.AddRange(repository.GetAll().Select(AsMessage));
            return Task.FromResult(reply);
        }

        public Task Handle(AddCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"command received with id {message.Id}");
            var newTodoItem = new TodoItem(Guid.Parse(message.Id), message.Title, message.DueDate.ToDateTime());
            repository.Add(newTodoItem);
            return Task.FromResult(new AddReply
            {
                Item = AsMessage(newTodoItem)
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
