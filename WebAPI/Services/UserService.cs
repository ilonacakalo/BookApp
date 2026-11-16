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
        var email = loginRequest.Email.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !_passwordService.Verify(loginRequest.Password, user.PasswordHash))
        {
            return null;
        }
        
        return user;
    }
}