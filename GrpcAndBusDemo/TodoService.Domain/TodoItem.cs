using System;

namespace TodoService.Domain
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public TodoItem(Guid id, string title, DateTime dueDate)
        {
            Id = id;
            Title = title;
            DueDate = dueDate;
        }
    }
}
