using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public enum InvitationStatus
    {
        NotInvited,
        Invited,
        Confirmed,
        Declined
    }
    public class EventGuest
    {
        [Key]
        public int EventGuestID { get; set; }

        [Required]
        public int GuestID { get; set; }
        public Guest Guest { get; set; }

        [Required]
        public int EventID { get; set; }
        public Event Event { get; set; }

        [Required]
        public InvitationStatus Status { get; set; }
    }
}
