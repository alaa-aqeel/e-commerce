using ECommerceApi.Application.DTOs.Review;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/reviews")]
public class ReviewsController : BaseController
{
    private readonly IReviewService _reviews;

    public ReviewsController(IReviewService reviews) => _reviews = reviews;

    /// <summary>Get reviews for a product</summary>
    [HttpGet("products/{productId}")]
    public async Task<IActionResult> GetProductReviews(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        HandleResult(await _reviews.GetProductReviewsAsync(productId, page, pageSize));

    /// <summary>Add a review to a product</summary>
    [HttpPost("products/{productId}")]
    [Authorize]
    public async Task<IActionResult> Create(int productId, [FromBody] CreateReviewRequest request) =>
        HandleResult(await _reviews.CreateAsync(CurrentUserId, productId, request));

    /// <summary>Update your own review</summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewRequest request) =>
        HandleResult(await _reviews.UpdateAsync(id, CurrentUserId, request));

    /// <summary>Delete your own review</summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id) =>
        HandleResult(await _reviews.DeleteAsync(id, CurrentUserId));
}
