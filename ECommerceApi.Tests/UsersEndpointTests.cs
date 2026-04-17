using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class UsersEndpointTests : IntegrationTestBase
{
    public UsersEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_AsAdmin_ReturnsSuccess()
    {
        await AuthenticateAsync("superadmin@test.com", "AdminPass123!", "Admin");
        
        var response = await Client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsync("normaluser@test.com", "CustomerPass123!", "Customer");
        
        var response = await Client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
