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
        private readonly ToDoContext _context;

        public ToDoController(ToDoContext context)
        {
            _context = context;
        }

        // Главната страница
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.TodoItems.Include(t => t.Category).ToListAsync();


            if (tasks == null)
            {
                tasks = new List<ToDoItem>(); // Уверяваме се, че списъкът с задачи не е null
            }

            return View(tasks); // Връщаме всички задачи на главната страница
        }


        // Създаване на нова задача
        [HttpGet]
        public IActionResult Create()
        {
            // Зареждаме категориите в ViewBag
            ViewBag.Categories = _context.Categories.ToList();
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItem model)
        {
           
                // Не трябва да задавате Id ръчно
                _context.TodoItems.Add(model);  // Стойността на Id ще бъде автоматично зададена от базата
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));  // Пренасочване към главната страница
           
        }

        // Редактиране на задача
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID не може да бъде празно.");
            }

            // Намираме задачата с конкретен ID
            var todoItem = await _context.TodoItems.FindAsync(id);

            // Ако не намерим задача с този ID, връщаме 404
            if (todoItem == null)
            {
                return NotFound($"Задача с ID {id} не е намерена.");
            }

            // Зареждаме категориите в ViewBag
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", todoItem.CategoryId);

            return View(todoItem);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DueDate,IsCompleted,CategoryId")] ToDoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(todoItem);  // Не задавайте Id ръчно, то ще бъде актуализирано автоматично
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));  // Пренасочване към главната страница
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

        // POST: Todo/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            _context.TodoItems.Remove(todoItem); // Премахваме задачата от базата
            await _context.SaveChangesAsync(); // Записваме промените
            return RedirectToAction(nameof(Index)); // Пренасочваме обратно към индекс
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

            todoItem.IsCompleted = true; // Променяме състоянието на задачата на завършена
            _context.Update(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // След завършване, връщаме към индекс
        }
    }
}
