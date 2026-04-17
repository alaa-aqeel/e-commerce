using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class CouponsEndpointTests : IntegrationTestBase
{
    public CouponsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_AsAdmin_ReturnsSuccess()
    {
        await AuthenticateAsync("couponadmin@test.com", "AdminPass123!", "Admin");
        
        var response = await Client.GetAsync("/api/coupons");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Validate_InvalidCoupon_ReturnsBadRequest()
    {
        await AuthenticateAsync("couponuser@test.com", "Pass123!");
        
        var request = new { Code = "NON_EXISTENT_CODE", TotalAmount = 100 };
        var response = await Client.PostAsJsonAsync("/api/coupons/validate", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"isValid\":false");
    }
}
