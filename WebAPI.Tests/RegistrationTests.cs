using System.Net;
using System.Net.Http.Json;
using WebAPI.DTOs;

namespace WebAPI.Tests;
public class RegistrationTests : IClassFixture<BookAppFactory>
{
    private readonly HttpClient _client;

    public RegistrationTests(BookAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static RegisterRequest CreateRequest(string username) => new()
    {
        Email = $"{username}@test.com",
        Username = username,
        Password = "Lozinka123"
    };

    [Fact]
    public async Task Register_ValidData_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("api/register", CreateRequest("ana"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_EmailAlreadyUsed_ReturnsConflict()
    {
        await _client.PostAsJsonAsync("api/register", CreateRequest("ivo"));

        var request = CreateRequest("ivo2");
        request.Email = "IVO@test.com";
        var response = await _client.PostAsJsonAsync("api/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("An account with this email already exists", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Register_UsernameAlreadyUsed_ReturnsConflict()
    {
        await _client.PostAsJsonAsync("api/register", CreateRequest("marko"));

        var request = CreateRequest("marko");
        request.Email = "other@test.com";
        var response = await _client.PostAsJsonAsync("api/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("An account with this username already exists", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("", "petra", "Lozinka123")]                  
    [InlineData("not-an-email", "petra", "Lozinka123")]      
    [InlineData("petra@test.com", "", "Lozinka123")]          
    [InlineData("petra@test.com", "pe", "Lozinka123")]       
    [InlineData("petra@test.com", "averyveryverylongname", "Lozinka123")] 
    [InlineData("petra@test.com", "petra", "")]             
    [InlineData("petra@test.com", "petra", "Short1A")]      
    [InlineData("petra@test.com", "petra", "lozinka123")]     
    [InlineData("petra@test.com", "petra", "LOZINKA123")]     
    [InlineData("petra@test.com", "petra", "LozinkaBez")]     
    public async Task Register_InvalidData_ReturnsBadRequest(string email, string username, string password)
    {
        var request = new RegisterRequest { Email = email, Username = username, Password = password };

        var response = await _client.PostAsJsonAsync("api/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_PasswordIsStoredHashed()
    {
        await _client.PostAsJsonAsync("api/register", CreateRequest("luka"));

        var response = await _client.PostAsJsonAsync("api/login",
            new LoginRequest { Username = "luka", Password = "Lozinka123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}