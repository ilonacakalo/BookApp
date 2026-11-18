using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Services;

public interface IUserService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
    Task<User?> LoginAsync(LoginRequest loginRequest);
    Task<(ProfileUpdateResult Result, User? User)> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<ChangePasswordResult> ChangePasswordAsync(int userId, ChangePasswordRequest request);
}