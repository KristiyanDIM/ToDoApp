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

        // Главната страница
        // Зареждаме всички задачи от базата данни с включени категории и потребители.
        // Ако няма задачи, създаваме празен списък.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Зарежда всички задачи от базата данни, категориите и потребителите
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
            // Зареждаме категориите чрез ViewBag
            ViewBag.Categories = _context.Categories.ToList();
            return View();

        }

        // Добавяме нова задача, създаваме потребител и свързваме задачата с потребителя.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItem model, string userName)
        {
           
            // Не трябва да задавате Id ръчно
            _context.TodoItems.Add(model);  
            await _context.SaveChangesAsync();

            //създаваме нов потребител
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
            // Проверка за празно ID
            if (id == null)
            {
                return BadRequest("ID не може да бъде празно.");
            }

            // зареждане на задачата с включени категории и потребители
            var todoItem = await _context.TodoItems
                                            .Include(t => t.UserTasks)
                                            .ThenInclude(ut => ut.User)
                                            .FirstOrDefaultAsync(t => t.Id == id);

            // Ако не намерим задача с този ID, връщаме 404
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


        // Актуализираме задачата и името на свързания потребител в базата данни.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DueDate,IsCompleted,CategoryId")] ToDoItem todoItem, string userName)
        {
            //Проверка за валидно ID
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            // Проверка за валидно потребителско име
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("UserName", "Името на потребителя е задължително.");
                return View(todoItem);
            }

            try
            {
                todoItem.UserName = userName;  // Присвояване на името на потребителя
                _context.Update(todoItem);
                await _context.SaveChangesAsync();

                // Намираме свързания потребител и актуализираме името
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

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        // Изтриване на задача
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            //проверка за празно ID
            if (id == null)
            {
                return NotFound();
            }

            //намира задачата и проверява дали съществува
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
            var todoItem = await _context.TodoItems.FindAsync(id); // Намираме задачата
            _context.TodoItems.Remove(todoItem); // Премахваме задачата от базата
            await _context.SaveChangesAsync(); // Записваме промените
            return RedirectToAction(nameof(Index)); 
        }

        // Отбелязване на задача като завършена
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            //намира задачата и проверява дали съществува
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.IsCompleted = true; // Променяме състоянието на задачата на завършена
            _context.Update(todoItem); // Обновяваме задачата
            await _context.SaveChangesAsync(); // Записваме промените
            return RedirectToAction(nameof(Index)); 
        }
    }
}
