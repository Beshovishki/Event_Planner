﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Автоматично генериране на стойност за EventID

        public int EventID { get; set; }
        [Required(ErrorMessage = "Моля, въведете име на събитието.")]
        public string EventName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Моля, въведете дата на събитието.")]
        public DateTime EventDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Моля, въведете място на събитието.")]
        public string EventPlace { get; set; } = string.Empty;
        [Required(ErrorMessage = "Моля, въведете описание събитието.")]
        public string Description { get; set; } = string.Empty;
        public ICollection<EventGuest> EventGuests { get; set; } = new List<EventGuest>();

        // Навигационно свойство за гостите (едно към много)
        public ICollection<Guest> Guests { get; set; } = new HashSet<Guest>();

        // Навигационно свойство за задачите (едно към много)
        public ICollection<EventTask> EventTasks { get; set; } = new HashSet<EventTask>();

        // Навигационно свойство за оценките (едно към много)
        public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();

        public bool IsArchived { get; set; } = false; // По подразбиране събитията не са архивирани (не се използва в момента)

        [NotMapped]
        public int GuestCount { get; set; }
    }
}