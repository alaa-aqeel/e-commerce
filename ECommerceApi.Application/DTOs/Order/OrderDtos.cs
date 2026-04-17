using ECommerceApi.Domain.Enums;

namespace ECommerceApi.Application.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public AddressSnapshotDto Address { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class AddressSnapshotDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string? VariantInfo { get; set; }
}

public class PlaceOrderRequest
{
    public int AddressId { get; set; }
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
}

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}

public class OrderFilterRequest
{
    public OrderStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
