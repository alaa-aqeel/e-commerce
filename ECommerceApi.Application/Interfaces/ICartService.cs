using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Cart;

namespace ECommerceApi.Application.Interfaces;

public interface ICartService
{
    Task<ServiceResult<CartDto>> GetCartAsync(int userId);
    Task<ServiceResult<CartDto>> AddItemAsync(int userId, AddToCartRequest request);
    Task<ServiceResult<CartDto>> UpdateItemAsync(int userId, int itemId, UpdateCartItemRequest request);
    Task<ServiceResult<bool>> RemoveItemAsync(int userId, int itemId);
    Task<ServiceResult<bool>> ClearCartAsync(int userId);
}
