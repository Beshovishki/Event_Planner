﻿using EventPlanner.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EventPlanner.Controllers.GuestController
{
    public class GuestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public GuestsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Guests
        
        public async Task<IActionResult> Index()
        {
            var guests = await _context.Guests.Include(g => g.EventGuests)
                .OrderBy(g => g.GuestName)
                .ToListAsync();
            return View(guests);
        }
        // GET: Guests/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("GuestName, Email, EventID")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        // GET: Guest/Edit/id
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return View(guest);
        }

        // POST: Guest/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [Bind("GuestID,GuestName,Email")] Guest guest)
        {
            if (id != guest.GuestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestExists(guest.GuestID))
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
            return View(guest);
        }

        // GET: Guests/Delete/id
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.GuestID == id);
            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // POST: Guests/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
                
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Guests/Invitation/5
        public IActionResult Invitation(int id)
        {
            // Взимане госта по id
            var guest = _context.Guests.FirstOrDefault(g => g.GuestID == id);

            if (guest == null)
            {
                return NotFound();
            }

            // Взимане всички събития, за да могат да се изберат
            var events = _context.Events
                    .Where(e => !_context.EventGuests.Any(eg => eg.EventID == e.EventID && eg.GuestID == guest.GuestID))
                    .ToList();
            ViewData["Events"] = new SelectList(events, "EventID", "EventName");

            return View(guest);  // Изпраща госта в изгледа
        }

        // POST: Guests/Invitation/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invitation(int id, [Bind("EventID")] Guest guest)
        {
            var guestToInvite = await _context.Guests.FindAsync(id);

            if (guestToInvite == null)
            {
                return NotFound();
            }

            // Проверка дали е избрано събитие
            if (guest.EventID == 0)
            {
                ModelState.AddModelError("EventID", "Моля, изберете събитие.");
                // Зареждаме отново събитията за да ги покажем в изгледа
                ViewData["Events"] = new SelectList(_context.Events, "EventID", "EventName");
                return View(guestToInvite); // Връщаме изгледа с грешката
            }

            // Добавяне на запис в EventGuests
            var eventGuest = new EventGuest
            {
                EventID = guest.EventID.Value,
                GuestID = guestToInvite.GuestID,
                Status = InvitationStatus.Invited // Статус "Поканен"
            };

            _context.EventGuests.Add(eventGuest);

            // Записваме промените в базата данни
            await _context.SaveChangesAsync();
            await SendInvitation(guestToInvite.GuestID);

            TempData["Success"] = "Поканата е изпратена успешно!";

            return RedirectToAction(nameof(Index));  // Пренасочваме към списъка с гости
        }

        public async Task<IActionResult> EventInvitations(int id) // или int guestId
        {
            // Зареждане на събитията, за които е поканен гостът
            var guestInvitations = await _context.EventGuests
                .Where(eg => eg.GuestID == id)
                .Include(eg => eg.Event)
                .ToListAsync();

            return View(guestInvitations);
        }
        

        [HttpPost]
        public async Task<IActionResult> ConfirmInvitation(int eventId, int guestId)
        {
            // Намираме връзката между събитието и госта
            var eventGuest = await _context.EventGuests
                .FirstOrDefaultAsync(eg => eg.EventID == eventId && eg.GuestID == guestId);

            if (eventGuest == null)
            {
                return NotFound();
            }

            // Променяме статуса на госта на "Приел поканата"
            eventGuest.Status = InvitationStatus.Confirmed;

            // Записваме промените в базата данни
            await _context.SaveChangesAsync();

            // Пренасочваме към изгледа за събитие
            return RedirectToAction("Index", "Guests");
        }
        public async Task<IActionResult> DeclineInvitation(int eventId, int guestId)
        {
            // Намираме връзката между събитието и госта
            var eventGuest = await _context.EventGuests
                .FirstOrDefaultAsync(eg => eg.EventID == eventId && eg.GuestID == guestId);

            if (eventGuest == null)
            {
                return NotFound();
            }

            // Изтриваме записа за поканата
            _context.EventGuests.Remove(eventGuest);

            // Записваме промените в базата данни
            await _context.SaveChangesAsync();

            // Пренасочваме към изгледа за събитие
            return RedirectToAction("Index", "Guests");
        }

        // POST: Guests/SendInvitation/id
        [HttpPost]
        public async Task<IActionResult> SendInvitation(int id)
        {
            var guest = await _context.Guests
                .Include(g => g.EventGuests) // Зареждаме връзката към събитията
                .ThenInclude(eg => eg.Event) // Взимаме и информацията за самото събитие
                .FirstOrDefaultAsync(g => g.GuestID == id);

            if (guest == null || guest.EventGuests == null || !guest.EventGuests.Any())
            {
                return NotFound("Гостът не е намерен или няма покани за събития.");
            }

            var eventDetails = guest.EventGuests.FirstOrDefault()?.Event;
            if (eventDetails == null)
            {
                return BadRequest("Гостът няма свързано събитие.");
            }

            // Създаваме имейл
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Event Organizer", _configuration["EmailSettings:SMTPUsername"]));
            emailMessage.To.Add(new MailboxAddress(guest.GuestName, guest.Email));
            emailMessage.Subject = $"Покана за събитието: {eventDetails.EventName}";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Здравейте {guest.GuestName},\n\n" +
                           $"Поканени сте на събитието: {eventDetails.EventName}.\n\n" +
                           $"Дата: {eventDetails.EventDate.ToString("dd/MM/yyyy - HH.mmч.")}\n" +
                           $"Място: {eventDetails.EventPlace}\n\n" +
                           $"Поздрави,\nЕкипът на Event Planner"
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // Конфигурация за SMTP
            var smtpServer = _configuration["EmailSettings:SMTPServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SMTPPort"]);
            var smtpUsername = _configuration["EmailSettings:SMTPUsername"];
            var smtpPassword = _configuration["EmailSettings:SMTPPassword"];

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(smtpUsername, smtpPassword);
                    await smtpClient.SendAsync(emailMessage);
                    await smtpClient.DisconnectAsync(true);
                }

                Console.WriteLine($"Поканата за {guest.GuestName} е изпратена успешно!");
                TempData["SuccessMessage"] = $"Поканата беше успешно изпратена на {guest.Email}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Грешка при изпращане на поканата: {ex.Message}");
                TempData["ErrorMessage"] = "Възникна грешка при изпращането на поканата.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GuestExists(int id)
        {
            return _context.Guests.Any(e => e.GuestID == id);
        }
    }
}
