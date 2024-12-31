using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// �������� �� ��������� �� ������ �����
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// �������� �� ������������ ������ �� MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ��������� �� ��������������� �� �����������
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ToDoContext>();
    ApplicationDbInitializer.Initialize(context); // ������������� �� �����������
}

// ������������� �� ��������
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // ���������� �� developer exception page �� ����������
}
else
{
    app.UseExceptionHandler("/ToDo/Error"); // �� ����� �� ������������ �� �� �������� handler
    app.UseHsts(); // ���������� �� HSTS
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
