using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ToDoApp.Data;
using ToDoApp.Models;


namespace ToDoApp.Controllers
{
    public class ToDoController : Controller
    {

        // Даваме възможност на контролера да работи с базата данни
        private readonly ToDoContext _context;

        public ToDoController(ToDoContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.TodoItems
                                         .Include(t => t.Category)
                                         .Include(t => t.UserTasks)
                                         .ThenInclude(ut => ut.User)
                                         .ToListAsync();

            // Създава празен списък, ако няма задачи
            if (tasks == null)
            {
                tasks = new List<ToDoItem>();
            }

            return View(tasks);
        }


        // Създаване на нова задача
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItem model, string userName)
        {

            _context.TodoItems.Add(model);
            await _context.SaveChangesAsync();

            var user = new Users { Name = userName };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //свързваме задачата с потребителя
            var userTask = new UserTask
            {
                UserId = user.Id,
                ToDoItemId = model.Id
            };

            _context.UserTasks.Add(userTask);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Редактиране на задача
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID не може да бъде празно.");
            }

            var todoItem = await _context.TodoItems
                                            .Include(t => t.UserTasks)
                                            .ThenInclude(ut => ut.User)
                                            .FirstOrDefaultAsync(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound($"Задача с ID {id} не е намерена.");
            }

            // Зареждаме категориите в ViewBag
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", todoItem.CategoryId);
            ViewBag.UserName = todoItem.UserTasks.FirstOrDefault()?.User?.Name ?? string.Empty;

            return View(todoItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DueDate,IsCompleted,CategoryId")] ToDoItem todoItem, string userName)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("UserName", "Името на потребителя е задължително.");
                return View(todoItem);
            }

            if (string.IsNullOrWhiteSpace(todoItem.Title))
            {
                ModelState.AddModelError("Title", "Заглавието на задачата е задължително.");
            }

            try
            {
                todoItem.UserName = userName;  // Присвояване на името на потребителя
                _context.Update(todoItem);
                await _context.SaveChangesAsync();

                // Намираме свързания потребител
                var userTask = await _context.UserTasks
                                            .Include(ut => ut.User)
                                            .FirstOrDefaultAsync(ut => ut.ToDoItemId == todoItem.Id);

                if (userTask != null)   
                {
                    userTask.User.Name = userName; // Обновяваме името на потребителя
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            // Проверка дали съществува задачата
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TodoItems.Any(e => e.Id == todoItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Проверка дали задачата съществува
        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }


        // Изтриване на задача
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }


        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Отбелязване на задача като завършена
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.IsCompleted = true; 
            _context.Update(todoItem); 
            await _context.SaveChangesAsync(); 
            return RedirectToAction(nameof(Index));
        }
    }
}
