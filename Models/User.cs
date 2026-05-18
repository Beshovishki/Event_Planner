namespace EventPlanner.Models
{
    public class AppUser
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
    }
}
