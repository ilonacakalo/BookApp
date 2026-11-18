using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(PasswordRules.MaxLength, MinimumLength = PasswordRules.MinLength, ErrorMessage = PasswordRules.LengthMessage)]
    [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.PatternMessage)]
    public string NewPassword { get; set; } = string.Empty;
}