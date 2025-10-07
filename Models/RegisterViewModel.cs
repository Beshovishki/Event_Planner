using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Потребителското име трябва да е между 3 и 100 символа")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Имейлът трябва да съдържа домейн (например .com, .bg и т.н.)")]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да е минимум 6 символа")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Потвърждението на паролата е задължително")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
