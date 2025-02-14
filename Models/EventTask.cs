namespace EventPlanner.Models
{
    public class EventTask
    {
        public int EventTaskID { get; set; }
        public string? TaskDescription { get; set; }
        public string? AssignedTo { get; set; }
        public string? TaskStatus { get; set; }

        // Връзка към събитие (много към едно)
        public int? EventID { get; set; }
        public Event? Event { get; set; }  // Навигационно свойство

        // Връзка към гост (много към едно)
        public int? GuestID { get; set; }
        public Guest? Guest { get; set; }  // Навигационно свойство
    }
}