namespace WebAPI.DTOs;

public static class PasswordRules
{
    public const int MinLength = 8;
    public const int MaxLength = 100;
    public const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";
    public const string LengthMessage = "Password must be at least 8 characters long";
    public const string PatternMessage = "Password must contain an uppercase letter, a lowercase letter and a number";
}