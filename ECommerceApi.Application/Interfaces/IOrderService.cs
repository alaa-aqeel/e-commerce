using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Order;

namespace ECommerceApi.Application.Interfaces;

public interface IOrderService
{
    Task<ServiceResult<PagedResult<OrderDto>>> GetUserOrdersAsync(int userId, int page, int pageSize);
    Task<ServiceResult<OrderDto>> GetByIdAsync(int id, int? userId = null);
    Task<ServiceResult<OrderDto>> PlaceOrderAsync(int userId, PlaceOrderRequest request);
    Task<ServiceResult<bool>> CancelOrderAsync(int orderId, int userId);
    Task<ServiceResult<PagedResult<OrderDto>>> GetAllOrdersAsync(OrderFilterRequest filter);
    Task<ServiceResult<OrderDto>> UpdateStatusAsync(int orderId, UpdateOrderStatusRequest request);
}
