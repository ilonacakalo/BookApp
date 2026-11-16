using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Services;

public interface IUserService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
    Task<User?> LoginAsync(LoginRequest loginRequest);
}