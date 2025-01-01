using Microsoft.Azure.Documents;

namespace ToDoApp.Models
{
    public class UserTask
    {
        public int UserId { get; set; }
        public Users User { get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }
    }

}
