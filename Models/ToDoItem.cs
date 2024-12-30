using System.ComponentModel.DataAnnotations;
using ToDoApp.Models;


namespace ToDoApp.Models

{
    public class ToDoItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Моля въведете заглавие.")]
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "Моля въведете дата.")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
}
