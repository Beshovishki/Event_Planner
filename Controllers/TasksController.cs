using System.Threading.Tasks;
using EventPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Controllers.TaskController
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EventTask

        public async Task<IActionResult> Index()
        {
            var tasks = await _context.EventTasks.Include(t => t.Event).ToListAsync();
            return View(tasks);
        }

        // GET: EventTask/Create
        public IActionResult Create()
        {
            ViewData["Events"] = new SelectList(_context.Events, "EventID", "EventName");
            return View();
        }

        // POST: EventTask/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskDescription,TaskStatus,EventID, AssignedTo")] EventTask eventTask)
        {
            if (ModelState.IsValid)
            {
                if (eventTask.TaskStatus == "Completed")
                {
                    eventTask.IsCompleted = true;
                }
                else
                {
                    eventTask.IsCompleted = false;
                }
                _context.Add(eventTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Events"] = new SelectList(_context.Events, "EventID", "EventName", eventTask.EventID);
            return View(eventTask);
        }

        // GET: EventTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTask = await _context.EventTasks.FindAsync(id);
            if (eventTask == null)
            {
                return NotFound();
            }

            ViewData["Events"] = new SelectList(_context.Events, "EventID", "EventName", eventTask.EventID);
            return View(eventTask);
        }

        // POST: EventTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventTaskID,TaskDescription,TaskStatus,EventID, AssignedTo")] EventTask eventTask)
        {
            if (id != eventTask.EventTaskID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ако статусът е "Completed", задаваме IsCompleted = true
                    if (eventTask.TaskStatus == "Completed")
                    {
                        eventTask.IsCompleted = true;
                    }
                    else
                    {
                        eventTask.IsCompleted = false;
                    }
                    _context.Update(eventTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventTaskExists(eventTask.EventTaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Events"] = new SelectList(_context.Events, "EventID", "EventName", eventTask.EventID);
            return View(eventTask);
        }
        // GET: Guests/Delete/5

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTask = await _context.EventTasks.FirstOrDefaultAsync(m => m.EventTaskID == id);
            if (eventTask == null)
            {
                return NotFound();
            }

            return View(eventTask);
        }
        // POST: EventTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventTask = await _context.EventTasks.FindAsync(id);
            if (eventTask != null)
            {
                _context.EventTasks.Remove(eventTask);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool EventTaskExists(int id)
        {
            return _context.EventTasks.Any(e => e.EventTaskID == id);
        }
    }
}