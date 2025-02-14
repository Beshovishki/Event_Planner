namespace EventPlanner.Models
{
    public class Guest
    {
        public int GuestID { get; set; }
        public int? EventID { get; set; }  // Foreign Key
        public string? GuestName { get; set; }
        public string? Email { get; set; }
        public string? RSVPStatus { get; set; }  // Поканен, Потвърден, Отказал

        // Връзка към събитие (много към едно)
       
        public Event? Event { get; set; }  // Навигационно свойство
    }
}