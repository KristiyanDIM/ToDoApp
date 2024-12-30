﻿using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;


namespace ToDoApp.Data
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDoApp.Models.ToDoItem> TodoItems { get; set; }
    }

   
}