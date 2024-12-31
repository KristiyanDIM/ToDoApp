using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Моля въведете име на категория.")]
        public string Name { get; set; } = string.Empty;

        // Свързани задачи
        public ICollection<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();
    }

}
