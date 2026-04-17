using ECommerceApi.Domain.Enums;

namespace ECommerceApi.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal Total { get; set; }
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
