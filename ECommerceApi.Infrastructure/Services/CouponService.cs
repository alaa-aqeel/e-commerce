using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Coupon;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CouponService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<List<CouponDto>>> GetAllAsync()
    {
        var coupons = await _context.Coupons.AsNoTracking().ToListAsync();
        return ServiceResult<List<CouponDto>>.Ok(_mapper.Map<List<CouponDto>>(coupons));
    }

    public async Task<ServiceResult<CouponDto>> CreateAsync(CreateCouponRequest request)
    {
        if (await _context.Coupons.AnyAsync(c => c.Code == request.Code))
            return ServiceResult<CouponDto>.Fail("Coupon code already exists.");

        var coupon = _mapper.Map<Coupon>(request);
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();
        return ServiceResult<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<ServiceResult<CouponDto>> UpdateAsync(int id, UpdateCouponRequest request)
    {
        var coupon = await _context.Coupons.FindAsync(id);
        if (coupon == null) return ServiceResult<CouponDto>.Fail("Coupon not found.");

        coupon.Type = request.Type;
        coupon.Value = request.Value;
        coupon.MinOrderAmount = request.MinOrderAmount;
        coupon.MaxUsageCount = request.MaxUsageCount;
        coupon.ExpiresAt = request.ExpiresAt;
        coupon.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return ServiceResult<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var coupon = await _context.Coupons.FindAsync(id);
        if (coupon == null) return ServiceResult<bool>.Fail("Coupon not found.");
        _context.Coupons.Remove(coupon);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<ValidateCouponResponse>> ValidateAsync(ValidateCouponRequest request)
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == request.Code && c.IsActive);
        if (coupon == null)
            return ServiceResult<ValidateCouponResponse>.Ok(new ValidateCouponResponse { IsValid = false, Message = "Invalid coupon code." });

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt < DateTime.UtcNow)
            return ServiceResult<ValidateCouponResponse>.Ok(new ValidateCouponResponse { IsValid = false, Message = "Coupon has expired." });

        if (coupon.MaxUsageCount.HasValue && coupon.UsageCount >= coupon.MaxUsageCount)
            return ServiceResult<ValidateCouponResponse>.Ok(new ValidateCouponResponse { IsValid = false, Message = "Coupon usage limit reached." });

        if (coupon.MinOrderAmount.HasValue && request.OrderAmount < coupon.MinOrderAmount)
            return ServiceResult<ValidateCouponResponse>.Ok(new ValidateCouponResponse
            {
                IsValid = false,
                Message = $"Minimum order amount is {coupon.MinOrderAmount:C}."
            });

        var discount = coupon.Type == Domain.Enums.CouponType.Percentage
            ? request.OrderAmount * coupon.Value / 100
            : coupon.Value;

        return ServiceResult<ValidateCouponResponse>.Ok(new ValidateCouponResponse
        {
            IsValid = true,
            Message = "Coupon applied successfully.",
            DiscountAmount = discount
        });
    }
}
