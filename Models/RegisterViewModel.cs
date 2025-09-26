//namespace EventPlanner.Models
//{
//    public class RegisterViewModel
//    {
//        public string Email { get; set; } = string.Empty;
//        public string Password { get; set; } = string.Empty;
//        public string ConfirmPassword { get; set; } = string.Empty;
//    }
//}

using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
