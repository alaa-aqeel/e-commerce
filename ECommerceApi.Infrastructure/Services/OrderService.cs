using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Order;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private IQueryable<Order> OrderQuery() =>
        _context.Orders
            .Include(o => o.User)
            .Include(o => o.Address)
            .Include(o => o.Items).ThenInclude(i => i.ProductVariant);

    public async Task<ServiceResult<PagedResult<OrderDto>>> GetUserOrdersAsync(int userId, int page, int pageSize)
    {
        var query = OrderQuery().AsNoTracking().Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt);
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ServiceResult<PagedResult<OrderDto>>.Ok(new PagedResult<OrderDto>
        {
            Items = _mapper.Map<List<OrderDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<OrderDto>> GetByIdAsync(int id, int? userId = null)
    {
        var query = OrderQuery().AsNoTracking().Where(o => o.Id == id);
        if (userId.HasValue) query = query.Where(o => o.UserId == userId.Value);

        var order = await query.FirstOrDefaultAsync();
        if (order == null) return ServiceResult<OrderDto>.Fail("Order not found.");
        return ServiceResult<OrderDto>.Ok(_mapper.Map<OrderDto>(order));
    }

    public async Task<ServiceResult<OrderDto>> PlaceOrderAsync(int userId, PlaceOrderRequest request)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.Items.Any())
            return ServiceResult<OrderDto>.Fail("Cart is empty.");

        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId);
        if (address == null) return ServiceResult<OrderDto>.Fail("Address not found.");

        // Check stock
        foreach (var item in cart.Items)
        {
            if (item.Product.Stock < item.Quantity)
                return ServiceResult<OrderDto>.Fail($"Insufficient stock for '{item.Product.Name}'.");
        }

        decimal subTotal = cart.Items.Sum(i => i.Quantity * (i.Product.DiscountPrice ?? i.Product.Price));
        decimal discount = 0;

        // Apply coupon
        if (!string.IsNullOrEmpty(request.CouponCode))
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == request.CouponCode && c.IsActive);
            if (coupon != null &&
                (coupon.ExpiresAt == null || coupon.ExpiresAt > DateTime.UtcNow) &&
                (coupon.MaxUsageCount == null || coupon.UsageCount < coupon.MaxUsageCount) &&
                (coupon.MinOrderAmount == null || subTotal >= coupon.MinOrderAmount))
            {
                discount = coupon.Type == Domain.Enums.CouponType.Percentage
                    ? subTotal * coupon.Value / 100
                    : coupon.Value;
                coupon.UsageCount++;
            }
        }

        var order = new Order
        {
            UserId = userId,
            AddressId = request.AddressId,
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            Discount = discount,
            Total = subTotal - discount,
            CouponCode = request.CouponCode,
            Notes = request.Notes,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity,
                UnitPrice = i.Product.DiscountPrice ?? i.Product.Price,
                ProductName = i.Product.Name,
                ProductImageUrl = i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.Url
                                  ?? i.Product.Images.FirstOrDefault()?.Url
            }).ToList()
        };

        // Deduct stock
        foreach (var item in cart.Items)
            item.Product.Stock -= item.Quantity;

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(order.Id);
    }

    public async Task<ServiceResult<bool>> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null) return ServiceResult<bool>.Fail("Order not found.");
        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            return ServiceResult<bool>.Fail("Order cannot be cancelled at this stage.");

        order.Status = OrderStatus.Cancelled;

        // Restore stock
        foreach (var item in order.Items)
            item.Product.Stock += item.Quantity;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<PagedResult<OrderDto>>> GetAllOrdersAsync(OrderFilterRequest filter)
    {
        var query = OrderQuery().AsNoTracking().OrderByDescending(o => o.CreatedAt);
        if (filter.Status.HasValue)
            query = (IOrderedQueryable<Order>)query.Where(o => o.Status == filter.Status.Value);

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return ServiceResult<PagedResult<OrderDto>>.Ok(new PagedResult<OrderDto>
        {
            Items = _mapper.Map<List<OrderDto>>(items),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    public async Task<ServiceResult<OrderDto>> UpdateStatusAsync(int orderId, UpdateOrderStatusRequest request)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return ServiceResult<OrderDto>.Fail("Order not found.");
        order.Status = request.Status;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(orderId);
    }
}
