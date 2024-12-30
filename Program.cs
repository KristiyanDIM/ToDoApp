using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// �������� �� ��������� �� ������ �����
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// �������� �� ������������ ������ �� MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ������������� �� ��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ToDo/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ������������� �� ���������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDo}/{action=Index}/{id?}"); // ��������� �� Home �� ToDo


// ���������� �� ������������
app.Run();
