using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class Guest
    {
        public int GuestID { get; set; }
        public int? EventID { get; set; }  // Foreign Key
        [Required(ErrorMessage = "Моля, въведете име на госта.")]
        public string GuestName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Моля, въведете Email на госта.")]
        public string Email { get; set; } = string.Empty;

        // Връзка към събитие (много към едно)

        public Event? Event { get; set; }  // Навигационно свойство
    }
}