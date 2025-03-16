using EventPlanner.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

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
            var guests = await _context.Guests.ToListAsync();
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

        // GET: Guest/Edit/5
        
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

        // POST: Guest/Edit/5
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

        // GET: Guests/Delete/5
        
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

        // POST: Guests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Guests/Invitation/5
        public IActionResult Invitation(int id)
        {
            // Вземете госта по ID
            var guest = _context.Guests.FirstOrDefault(g => g.GuestID == id);

            if (guest == null)
            {
                return NotFound();
            }

            // Предаваме списъка със събития на изгледа
            var events = _context.Events.ToList();  // Зареждате събитията
            ViewData["Events"] = new SelectList(_context.Events, "EventID", "EventName");
            return View(guest);  // Тук подавате госта, за да го имате в изгледа
        }

        // POST: Guests/Invitation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invitation(int id, [Bind("EventID", "GuestID", "GuestName")] Guest guest)
        {
            var guestToInvite = await _context.Guests.FindAsync(id);

            if (guestToInvite == null)
            {
                return NotFound();
            }

            // Свързваме избраното събитие с госта
            guestToInvite.EventID = guest.EventID;

            // Тук добавяте логика за изпращане на покана (по имейл или друг начин)
            // Изпращане на имейл или друго действие за поканата...

            // Записваме промените
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  // Пренасочване към списъка с гости
        }

        // POST: Guests/SendInvitation/5
        [HttpPost]
        public async Task<IActionResult> SendInvitation(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }

            // Изпращане на имейл с MailKit
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Event Organizer", "your-email@example.com"));
            emailMessage.To.Add(new MailboxAddress(guest.GuestName, guest.Email));
            emailMessage.Subject = "Invitation to Event";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Dear {guest.GuestName},\nYou are invited to the event: {guest.Event.EventName}.\n\nBest regards,\nEvent Planner"
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // Получаване на конфигурация от appsettings.json
            var smtpServer = _configuration["EmailSettings:SMTPServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SMTPPort"]);
            var smtpUsername = _configuration["EmailSettings:SMTPUsername"];
            var smtpPassword = _configuration["EmailSettings:SMTPPassword"];

            // Изпращане на имейла
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_configuration["EmailSettings:SmtpServer"],
                                             int.Parse(_configuration["EmailSettings:Port"]),
                                             bool.Parse(_configuration["EmailSettings:UseSSL"]));

                await smtpClient.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }

            Console.WriteLine($"Поканата за {guest.GuestName} е изпратена!");

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmAttendance(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();  // Ако гостът не е намерен, връща 404
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Guests");
        }
        private bool GuestExists(int id)
        {
            return _context.Guests.Any(e => e.GuestID == id);
        }
    }
}
