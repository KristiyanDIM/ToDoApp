using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

namespace ToDoApp.Data
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }

        public DbSet<ToDoItem> TodoItems { get; set; }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Users> Users { get; set; } 
        public DbSet<UserTask> UserTasks { get; set; } // Свързваща таблица

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Връзка между UserTask и другите модели
            modelBuilder.Entity<UserTask>()
                .HasKey(ut => new { ut.UserId, ut.ToDoItemId });

            //Връзка между UserTask и Users
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId);

            //Връзка между UserTask и TodoItem
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.ToDoItem)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.ToDoItemId);

            // Връзка между ToDoItem и Category
            modelBuilder.Entity<ToDoItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.ToDoItems)
                .HasForeignKey(t => t.CategoryId);
        }
    }
}
