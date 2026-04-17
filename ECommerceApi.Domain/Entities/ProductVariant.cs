namespace ECommerceApi.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public string Name { get; set; } = string.Empty;  // e.g. "Size", "Color"
    public string Value { get; set; } = string.Empty; // e.g. "XL", "Red"
    public decimal? PriceAdjustment { get; set; }
    public int StockAdjustment { get; set; } = 0;
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
}
