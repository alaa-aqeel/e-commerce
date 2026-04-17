using System.Net;
using FluentAssertions;

namespace ECommerceApi.Tests;

public class ReviewsEndpointTests : IntegrationTestBase
{
    public ReviewsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetProductReviews_ReturnsSuccess()
    {
        var response = await Client.GetAsync("/api/reviews/products/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateReview_UnauthenticatedUser_ReturnsUnauthorized()
    {
        var request = new { Rating = 5, Comment = "Great product!" };
        var response = await Client.PostAsJsonAsync("/api/reviews/products/1", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
