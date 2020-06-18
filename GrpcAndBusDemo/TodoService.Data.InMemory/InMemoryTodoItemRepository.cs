using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TodoService.Domain;

namespace TodoService.Data.InMemory
{
    public class InMemoryTodoItemRepository : ITodoItemRepository
    {
        private ConcurrentDictionary<Guid, TodoItem> items = new ConcurrentDictionary<Guid, TodoItem>();
        public void Add(TodoItem todoItem)
        {
            if (!items.TryAdd(todoItem.Id, todoItem))
                throw new Exception($"Unable to add todo with id {todoItem.Id}");
        }

        public ICollection<TodoItem> GetAll()
        {
            return items.Values;
        }
    }
}
