namespace EventPlanner.Models
{

    public class Rating
    {
        public int RatingID { get; set; }
        public int? RatingValue { get; set; }
        public string? Comments { get; set; }

        // Връзка към събитие (много към едно)
        public int? EventID { get; set; }
        public Event? Event { get; set; }  // Навигационно свойство
    }
}