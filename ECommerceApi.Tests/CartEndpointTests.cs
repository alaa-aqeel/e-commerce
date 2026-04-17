using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class CartEndpointTests : IntegrationTestBase
{
    public CartEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetCart_AuthenticatedUser_ReturnsEmptyCartOrSuccess()
    {
        await AuthenticateAsync("cartuser@test.com", "Pass123!");
        
        var response = await Client.GetAsync("/api/cart");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddItemToCart_NoProduct_ReturnsBadRequest()
    {
        await AuthenticateAsync("cartuser2@test.com", "Pass123!");
        
        // Product 99999 doesn't exist
        var request = new { ProductId = 99999, Quantity = 1 };
        var response = await Client.PostAsJsonAsync("/api/cart/items", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
