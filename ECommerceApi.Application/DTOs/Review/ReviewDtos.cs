namespace ECommerceApi.Application.DTOs.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserImageUrl { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class UpdateReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
