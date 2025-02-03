// ApplicationDbInitializer.cs
using System.Collections.Generic;
using System.Linq;
using ToDoApp.Data;
using ToDoApp.Models;

public class ApplicationDbInitializer
{
    public static void Initialize(ToDoContext context)
    {
        context.Database.EnsureCreated();  // Създава базата, ако не съществува.

        if (context.Categories.Any())
        {
            return; 
        }

       
        var categories = new List<Category>
        {
            new Category { Name = "Лични задачи" },
            new Category { Name = "Работа" },
            new Category { Name = "Покупки" },
            new Category { Name = "Здраве" },
            new Category { Name = "Учебни задачи" },
            new Category { Name = "Семейни задачи" },
            new Category { Name = "Пътувания" },
            new Category { Name = "Хоби" },
            new Category { Name = "Домакинство" },
            new Category { Name = "Проекти" }
        };

        context.Categories.AddRange(categories);  // Добавя всички категории в контекста
        context.SaveChanges();  
    }
}
