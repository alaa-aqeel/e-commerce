using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Review;

namespace ECommerceApi.Application.Interfaces;

public interface IReviewService
{
    Task<ServiceResult<PagedResult<ReviewDto>>> GetProductReviewsAsync(int productId, int page, int pageSize);
    Task<ServiceResult<ReviewDto>> CreateAsync(int userId, int productId, CreateReviewRequest request);
    Task<ServiceResult<ReviewDto>> UpdateAsync(int reviewId, int userId, UpdateReviewRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int reviewId, int userId);
}
