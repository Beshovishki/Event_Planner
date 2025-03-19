using Microsoft.EntityFrameworkCore;
using EventPlanner.Models;

namespace EventPlanner.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<EventTask> EventTasks { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<EventGuest> EventGuests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Извикване на base метода

            // Конфигурира връзката между Guest и Event
            modelBuilder.Entity<Guest>()
                .HasOne(g => g.Event)
                .WithMany(e => e.Guests)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурира връзката между EventTask и Event
            modelBuilder.Entity<EventTask>()
                .HasOne(et => et.Event)
                .WithMany(e => e.EventTasks)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурира връзката между Rating и Event
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Ratings)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурира връзката между EventGuest и Guest
            modelBuilder.Entity<EventGuest>()
                .HasOne(eg => eg.Guest)
                .WithMany(g => g.EventGuests)
                .HasForeignKey(eg => eg.GuestID)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурира връзката между EventGuest и Event
            modelBuilder.Entity<EventGuest>()
                .HasOne(eg => eg.Event)
                .WithMany(e => e.EventGuests)
                .HasForeignKey(eg => eg.EventID)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
