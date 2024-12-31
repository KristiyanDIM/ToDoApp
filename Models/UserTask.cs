using Microsoft.Azure.Documents;

namespace ToDoApp.Models
{
    public class UserTask
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }
    }

}
