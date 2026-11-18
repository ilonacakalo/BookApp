using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Services;

public class UserService : IUserService
{
    private readonly BookAppContext _context;
    private readonly IPasswordService _passwordService;
    
    public UserService(BookAppContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest)
    {
        var email = registerRequest.Email.Trim().ToLowerInvariant();
        var username = registerRequest.Username.Trim();

        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return RegisterResult.EmailTaken;
        }

        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return RegisterResult.UsernameTaken;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordService.Hash(registerRequest.Password)
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return RegisterResult.Success;
    }

    public async Task<User?> LoginAsync(LoginRequest loginRequest)
    {
        var username = loginRequest.Username.Trim();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !_passwordService.Verify(loginRequest.Password, user.PasswordHash))
        {
            return null;
        }
        
        return user;
    }
    public async Task<(ProfileUpdateResult Result, User? User)> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return (ProfileUpdateResult.NotFound, null);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        if (await _context.Users.AnyAsync(u => u.Id != userId && u.Email == email))
        {
            return (ProfileUpdateResult.EmailTaken, null);
        }

        if (await _context.Users.AnyAsync(u => u.Id != userId && u.Username == username))
        {
            return (ProfileUpdateResult.UsernameTaken, null);
        }

        user.Email = email;
        user.Username = username;
        await _context.SaveChangesAsync();
        return (ProfileUpdateResult.Success, user);
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ChangePasswordResult.NotFound;
        }

        if (!_passwordService.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return ChangePasswordResult.WrongPassword;
        }

        // BCrypt generates a new salt on every Hash call
        user.PasswordHash = _passwordService.Hash(request.NewPassword);
        await _context.SaveChangesAsync();
        return ChangePasswordResult.Success;
    }
}