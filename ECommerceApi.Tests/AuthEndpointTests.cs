using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class AuthEndpointTests : IntegrationTestBase
{
    public AuthEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Register_ValidUser_ReturnsSuccess()
    {
        var request = new { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com", Password = "Password123!", ConfirmPassword = "Password123!" };
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"success\":true");
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        await AuthenticateAsync("jane.doe@test.com", "Password123!");
        
        var request = new { Email = "jane.doe@test.com", Password = "Password123!" };
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("accessToken");
    }
}
