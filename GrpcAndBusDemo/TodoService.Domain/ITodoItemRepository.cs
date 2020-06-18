using System.Collections.Generic;

namespace TodoService.Domain
{
    public interface ITodoItemRepository
    {
        ICollection<TodoItem> GetAll();
        void Add(TodoItem todoItem);
    }
}
