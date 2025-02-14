using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Автоматично генериране на стойност за EventID

        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventPlace { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Навигационно свойство за гостите (едно към много)
        public ICollection<Guest> Guests { get; set; } = new HashSet<Guest>();

        // Навигационно свойство за задачите (едно към много)
        public ICollection<EventTask> EventTasks { get; set; } = new HashSet<EventTask>();

        // Навигационно свойство за оценките (едно към много)
        public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
    }
}