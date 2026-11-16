using System.Net;
using System.Net.Http.Json;
using WebAPI.DTOs;

namespace WebAPI.Tests;

public class LoginLogoutTests : IClassFixture<BookAppFactory>
{
    private readonly BookAppFactory _factory;

    public LoginLogoutTests(BookAppFactory factory)
    {
        _factory = factory;
    }
    private async Task<HttpClient> CreateClientWithUserAsync(string username)
    {
        var client = _factory.CreateClient();

        await client.PostAsJsonAsync("api/register", new RegisterRequest
        {
            Email = $"{username}@test.com",
            Username = username,
            Password = "Lozinka123"
        });

        return client;
    }

    [Fact]
    public async Task Login_CorrectData_ReturnsUserAndSetsCookie()
    {
        var client = await CreateClientWithUserAsync("ana");

        var response = await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "ana", Password = "Lozinka123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(user);
        Assert.Equal("ana", user.Username);
        Assert.Equal("ana@test.com", user.Email);
        Assert.Equal("Student", user.Role);

        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
        Assert.Contains(cookies, c => c.StartsWith("BookApp.Auth=") && c.Contains("httponly"));
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var client = await CreateClientWithUserAsync("ivo");

        var response = await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "ivo", Password = "Wrong1234" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Invalid username or password", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Login_UnknownUsername_ReturnsSameMessageAsWrongPassword()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "nobody", Password = "Lozinka123" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Invalid username or password", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("", "Lozinka123")] // username missing
    [InlineData("ana", "")]        // password missing
    public async Task Login_MissingData_ReturnsBadRequest(string username, string password)
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = username, Password = password });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithoutLogin_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_AfterLogin_ReturnsLoggedInUser()
    {
        var client = await CreateClientWithUserAsync("marko");
        await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "marko", Password = "Lozinka123" });

        var response = await client.GetAsync("api/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.Equal("marko", user!.Username);
    }

    [Fact]
    public async Task Logout_AfterLogin_EndsTheSession()
    {
        var client = await CreateClientWithUserAsync("petra");
        await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "petra", Password = "Lozinka123" });

        var logoutResponse = await client.PostAsync("api/logout", null);
        var meResponse = await client.GetAsync("api/me");

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, meResponse.StatusCode);
    }

    [Fact]
    public async Task Login_RememberMe_CreatesPersistentCookie()
    {
        var client = await CreateClientWithUserAsync("luka");

        var response = await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "luka", Password = "Lozinka123", RememberMe = true });

        var cookie = response.Headers.GetValues("Set-Cookie").First(c => c.StartsWith("BookApp.Auth="));
        Assert.Contains("expires=", cookie);
    }

    [Fact]
    public async Task Login_WithoutRememberMe_CreatesSessionCookie()
    {
        var client = await CreateClientWithUserAsync("maja");

        var response = await client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "maja", Password = "Lozinka123", RememberMe = false });

        var cookie = response.Headers.GetValues("Set-Cookie").First(c => c.StartsWith("BookApp.Auth="));
        Assert.DoesNotContain("expires=", cookie);
    }
}