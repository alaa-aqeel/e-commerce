using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Coupon;

namespace ECommerceApi.Application.Interfaces;

public interface ICouponService
{
    Task<ServiceResult<List<CouponDto>>> GetAllAsync();
    Task<ServiceResult<CouponDto>> CreateAsync(CreateCouponRequest request);
    Task<ServiceResult<CouponDto>> UpdateAsync(int id, UpdateCouponRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<ValidateCouponResponse>> ValidateAsync(ValidateCouponRequest request);
}
