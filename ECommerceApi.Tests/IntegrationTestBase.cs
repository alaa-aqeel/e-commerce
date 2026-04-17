using System.Net.Http.Headers;
using ECommerceApi.Application.DTOs.Auth;
using Microsoft.Extensions.DependencyInjection;
using ECommerceApi.Infrastructure.Persistence;

namespace ECommerceApi.Tests;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    public IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Authenticates a test user and adds the JWT bearer token to the default request headers.
    /// It first tries to log in, and if the user doesn't exist, it registers them.
    /// </summary>
    protected async Task AuthenticateAsync(string email = "test@example.com", string password = "Password123!", string role = "Customer")
    {
        // Check if user exists via Auth endpoint (Login)
        var loginRequest = new LoginRequest { Email = email, Password = password };
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            // Create user via Register endpoint
            var registerRequest = new RegisterRequest 
            { 
                FirstName = "Test", 
                LastName = "User", 
                Email = email, 
                Password = password
            };
            var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
            registerResponse.EnsureSuccessStatusCode();

            // Note: Currently the application defines Role locally, but for Admin roles, we might need a direct DB override
            if (role == "Admin")
            {
                using var scope = Factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    user.Role = Domain.Enums.UserRole.Admin;
                    await db.SaveChangesAsync();
                }
            }

            // Retry Login
            response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        }

        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();
        var result = System.Text.Json.JsonDocument.Parse(jsonString).RootElement;
        
        var data = result.GetProperty("data");
        if (!data.TryGetProperty("accessToken", out var tokenProp) && !data.TryGetProperty("AccessToken", out tokenProp))
        {
            throw new InvalidOperationException($"Token not found in response payload: {jsonString}");
        }
        var token = tokenProp.GetString();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
