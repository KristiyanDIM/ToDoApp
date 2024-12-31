using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавяне на контекста на базата данни
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавяне на необходимите услуги за MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Извикване на инициализацията на категориите
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ToDoContext>();
    ApplicationDbInitializer.Initialize(context); // Инициализация на категориите
}

// Конфигуриране на заявките
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Използване на developer exception page за разработка
}
else
{
    app.UseExceptionHandler("/ToDo/Error"); // По време на производство да се използва handler
    app.UseHsts(); // Активиране на HSTS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Конфигуриране на маршрутизацията
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDo}/{action=Index}/{id?}"); // Променете от Home на ToDo

// Стартиране на приложението
app.Run();
