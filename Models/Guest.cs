namespace EventPlanner.Models
{
    public class Guest
    {
        public int GuestID { get; set; }
        public int? EventID { get; set; }  // Foreign Key
        public string GuestName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RSVPStatus { get; set; } = string.Empty; // Поканен, Потвърден, Отказал

        // Връзка към събитие (много към едно)

        public Event? Event { get; set; }  // Навигационно свойство
    }
}