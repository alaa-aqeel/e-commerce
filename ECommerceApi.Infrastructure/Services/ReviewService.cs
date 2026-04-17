using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Review;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReviewService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<ReviewDto>>> GetProductReviewsAsync(int productId, int page, int pageSize)
    {
        var query = _context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ServiceResult<PagedResult<ReviewDto>>.Ok(new PagedResult<ReviewDto>
        {
            Items = _mapper.Map<List<ReviewDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<ReviewDto>> CreateAsync(int userId, int productId, CreateReviewRequest request)
    {
        if (await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId))
            return ServiceResult<ReviewDto>.Fail("You have already reviewed this product.");

        if (request.Rating < 1 || request.Rating > 5)
            return ServiceResult<ReviewDto>.Fail("Rating must be between 1 and 5.");

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        await _context.Entry(review).Reference(r => r.User).LoadAsync();
        return ServiceResult<ReviewDto>.Ok(_mapper.Map<ReviewDto>(review));
    }

    public async Task<ServiceResult<ReviewDto>> UpdateAsync(int reviewId, int userId, UpdateReviewRequest request)
    {
        var review = await _context.Reviews.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

        if (review == null) return ServiceResult<ReviewDto>.Fail("Review not found.");

        review.Rating = request.Rating;
        review.Comment = request.Comment;
        await _context.SaveChangesAsync();
        return ServiceResult<ReviewDto>.Ok(_mapper.Map<ReviewDto>(review));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int reviewId, int userId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
        if (review == null) return ServiceResult<bool>.Fail("Review not found.");
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
