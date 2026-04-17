using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class CategoriesEndpointTests : IntegrationTestBase
{
    public CategoriesEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await Client.GetAsync("/api/categories");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCategory_AsAdmin_ReturnsSuccess()
    {
        await AuthenticateAsync("admin@test.com", "AdminPass123!", "Admin");
        
        var request = new { Name = "Electronics", Description = "Tech stuff" };
        var response = await Client.PostAsJsonAsync("/api/categories", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCategory_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsync("customer@test.com", "CustomerPass123!", "Customer");
        
        var request = new { Name = "Books", Description = "Reading material" };
        var response = await Client.PostAsJsonAsync("/api/categories", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
