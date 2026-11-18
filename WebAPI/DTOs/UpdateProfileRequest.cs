using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [StringLength(16, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 16 characters")]
    public string Username { get; set; } = string.Empty;
}