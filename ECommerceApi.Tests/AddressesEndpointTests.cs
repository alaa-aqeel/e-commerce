using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class AddressesEndpointTests : IntegrationTestBase
{
    public AddressesEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetMyAddresses_AuthenticatedUser_ReturnsSuccess()
    {
        await AuthenticateAsync("addressuser@test.com", "Pass123!");
        
        var response = await Client.GetAsync("/api/addresses");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAddress_AuthenticatedUser_ReturnsSuccess()
    {
        await AuthenticateAsync("addressuser@test.com", "Pass123!");
        
        var request = new { 
            Street = "123 Main St", 
            City = "New York", 
            State = "NY", 
            ZipCode = "10001", 
            Country = "USA", 
            IsDefault = true 
        };
        var response = await Client.PostAsJsonAsync("/api/addresses", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
