using System.ComponentModel.DataAnnotations;
using ToDoApp.Models;


namespace ToDoApp.Models

{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
        public ICollection<UserTask> UserTasks { get; set; } // Add this property
        public string UserName { get; set; } // Add this property


    }
}
