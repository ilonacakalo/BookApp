using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [StringLength(16, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 16 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(PasswordRules.MaxLength, MinimumLength = PasswordRules.MinLength, ErrorMessage = PasswordRules.LengthMessage)]
    [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.PatternMessage)]
    public string Password { get; set; } = string.Empty;
}