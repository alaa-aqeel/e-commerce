using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class NotificationsEndpointTests : IntegrationTestBase
{
    public NotificationsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetMyNotifications_AuthenticatedUser_ReturnsSuccess()
    {
        await AuthenticateAsync("notificationuser@test.com", "Pass123!");
        
        var response = await Client.GetAsync("/api/notifications");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MarkAsRead_NonExistentNotification_ReturnsBadRequest()
    {
        await AuthenticateAsync("notificationuser@test.com", "Pass123!");
        
        var response = await Client.PutAsync("/api/notifications/99999/read", null);
        
        // Either BadRequest (NotFound message) or NotFound depending on standardizing
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 
    }
}
