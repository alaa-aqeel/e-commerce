namespace ECommerceApi.Domain.Entities;

public class Review : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
