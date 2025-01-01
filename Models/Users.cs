using System.Collections.Generic;

namespace ToDoApp.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Добавете тази колекция, ако липсва
        public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}
