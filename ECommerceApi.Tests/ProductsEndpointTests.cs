using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class ProductsEndpointTests : IntegrationTestBase
{
    public ProductsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await Client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct_AsAdmin_ReturnsSuccess()
    {
        await AuthenticateAsync("admin2@test.com", "AdminPass123!", "Admin");
        
        // Ensure a category exists
        var catResponse = await Client.PostAsJsonAsync("/api/categories", new { Name = "Cat1", Description = "Desc" });
        var catContent = await catResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        int categoryId = catContent.GetProperty("data").GetProperty("id").GetInt32();

        var request = new { 
            Name = "Laptop", 
            Description = "Fast PC",
            Price = 999.99m,
            StockQuantity = 10,
            CategoryId = categoryId
        };
        var response = await Client.PostAsJsonAsync("/api/products", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
