using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class OrdersEndpointTests : IntegrationTestBase
{
    public OrdersEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetMyOrders_AuthenticatedUser_ReturnsSuccess()
    {
        await AuthenticateAsync("orderuser@test.com", "Pass123!");
        
        var response = await Client.GetAsync("/api/orders");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task PlaceOrder_EmptyCart_ReturnsBadRequest()
    {
        await AuthenticateAsync("orderuser2@test.com", "Pass123!");
        
        var request = new { AddressId = 1 };
        var response = await Client.PostAsJsonAsync("/api/orders", request);
        
        // This should fail because cart is empty/address is invalid
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
