﻿using System.ComponentModel.DataAnnotations;
using EventPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events

        public async Task<IActionResult> Index(bool showArchived = false)
        {
            // Ако е избрано да се показват архивираните събития
            if (showArchived)
            {
                var archivedEvents = await _context.Events
                    .Where(e => e.IsArchived == true) // Филтрираме само архивираните събития
                    .Include(e => e.Ratings) // Зареждаме всички оценки за всяко събитие
                    .ToListAsync();

                return View(archivedEvents);
            }
            else
            {
                var events = await _context.Events
                    .Where(e => e.EventDate >= DateTime.Now || (e.EventDate.AddDays(5) >= DateTime.Now && e.IsArchived == false)) // Включва бъдещи събития и събития, които не са архивирани
                    .Include(e => e.Ratings) // Зареждаме всички оценки за всяко събитие
                    .ToListAsync();

                return View(events);
            }


            //// Ако е избрано да се показват архивираните събития
            //if (showArchived)
            //{
            //    var archivedEvents = await _context.Events
            //        .Where(e => e.EventDate.AddDays(5) < DateTime.Now) // Примерна логика: събития с дата в миналото
            //        .Include(e => e.Ratings) // Зарежда всички оценки за всяко събитие
            //        .ToListAsync();

            //    return View(archivedEvents);
            //}
            //else
            //{
            //    var activeEvents = await _context.Events
            //        .Where(e => e.EventDate >= DateTime.Now || e.EventDate.AddDays(5) >= DateTime.Now) // Включва бъдещи събития и събития до 5 дни назад
            //        .Include(e => e.Ratings) // Зарежда всички оценки за всяко събитие
            //        .ToListAsync();

            //    return View(activeEvents);
            //}
        }
        public async Task ArchiveEvents()
        {
            var eventsToArchive = await _context.Events
                .Where(e => e.EventDate.AddDays(5) < DateTime.Now && !e.IsArchived) // Събития, които са преминали 5 дни и не са архивирани
                .ToListAsync();

            foreach (var eventItem in eventsToArchive)
            {
                eventItem.IsArchived = true; // Маркираме събитието като архивирано
            }

            await _context.SaveChangesAsync(); // Записваме промените в базата данни
        }
        public async Task<IActionResult> Archive()
        {
            // Зареждаме събития, които са преминали повече от 5 дни
            var archivedEvents = await _context.Events
                .Where(e => e.EventDate.AddDays(5) < DateTime.Now) // Събития, които са завършили преди повече от 5 дни
                .Include(e => e.Ratings) // Зареждаме оценките
                .ToListAsync();

            return View(archivedEvents); // Връщаме към изгледа за архивираните събития
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDate,EventPlace,Description")] Event eventModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(eventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventModel);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,EventDate,EventPlace,Description")] Event eventModel)
        {
            if (id != eventModel.EventID)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventModel.EventID))
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
            return View(eventModel);
        }
        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);

            if (eventModel == null)
            {
                return NotFound(); // Връща 404 грешка, ако събитието не е намерено
            }

            _context.Events.Remove(eventModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Rate/5
        public IActionResult Rate(int id)
        {
            var eventModel = _context.Events.Find(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(new Rating { EventID = id });
        }

        // POST: Events/Rate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int id, [Bind("RatingValue, Comments")] Rating rating)
        {
            var eventModel = _context.Events.Find(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                rating.EventID = id;
                _context.Ratings.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));  // Пренасочване след успешен запис
            }

            return View(rating);  // Връща формата за повторно попълване, ако има грешки
        }
        public async Task<IActionResult> Reports(DateTime? startDate, DateTime? endDate, string? location)
        {
            var query = _context.Events
                .Include(e => e.Guests)
                .Include(e => e.EventTasks)
                .Include(e => e.Ratings)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(e => e.EventDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.EventDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.EventPlace.Contains(location));
            }

            var events = await query.ToListAsync();
            return View(events);
        }
        public int GetVoteCount(int eventId)
        {
            return _context.Ratings.Count(r => r.EventID == eventId);
        }
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}
