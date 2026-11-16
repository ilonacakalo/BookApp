using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly HttpClient _http;
    private AuthenticationState? _currentState;

    public AuthStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _currentState ??= await LoadCurrentUserAsync();
        return _currentState;
    }

    public async Task<string?> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/login", loginRequest);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Invalid username or password";
            }

            if (!response.IsSuccessStatusCode)
            {
                return "Login failed. Please try again.";
            }

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            SetState(user == null ? Anonymous : CreateState(user));
            return null;
        }
        catch (HttpRequestException)
        {
            return "Can't reach the server. Please try again later.";
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _http.PostAsync("api/logout", null);
        }
        catch (HttpRequestException)
        { }

        SetState(Anonymous);
    }

    private async Task<AuthenticationState> LoadCurrentUserAsync()
    {
        try
        {
            var user = await _http.GetFromJsonAsync<UserResponse>("api/me");
            return user == null ? Anonymous : CreateState(user);
        }
        catch (HttpRequestException)
        {
            return Anonymous;
        }
    }

    private void SetState(AuthenticationState state)
    {
        _currentState = state;
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    private static AuthenticationState CreateState(UserResponse user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}