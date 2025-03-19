using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class EventTask
    {
        public int EventTaskID { get; set; }
        [Required(ErrorMessage = "Моля, въведете описание на задачата.")]
        public string TaskDescription { get; set; } = string.Empty;
        [Required(ErrorMessage = "Моля, въведете отговорник.")]
        public string? AssignedTo { get; set; } = string.Empty;
        public string? TaskStatus { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;

        // Връзка към събитие (много към едно)
        [Required(ErrorMessage = "Моля, въведете събитие.")]
        public int? EventID { get; set; }
        public Event? Event { get; set; }  // Навигационно свойство

        // Връзка към гост (много към едно)
        public int? GuestID { get; set; }
        public Guest? Guest { get; set; }  // Навигационно свойство
    }
}