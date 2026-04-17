using AutoMapper;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.DTOs.Category;
using ECommerceApi.Application.DTOs.Product;
using ECommerceApi.Application.DTOs.Cart;
using ECommerceApi.Application.DTOs.Order;
using ECommerceApi.Application.DTOs.Review;
using ECommerceApi.Application.DTOs.Coupon;
using ECommerceApi.Application.DTOs.Address;
using ECommerceApi.Application.DTOs.Notification;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        // Category
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null));
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<UpdateCategoryRequest, Category>();

        // Product
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0))
            .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.Reviews.Count));
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<ProductVariant, ProductVariantDto>();
        CreateMap<CreateProductRequest, Product>();

        // Cart
        CreateMap<Cart, CartDto>()
            .ForMember(d => d.SubTotal, o => o.MapFrom(s =>
                s.Items.Sum(i => i.Quantity * (i.Product.DiscountPrice ?? i.Product.Price))))
            .ForMember(d => d.TotalItems, o => o.MapFrom(s => s.Items.Sum(i => i.Quantity)));
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductImageUrl, o => o.MapFrom(s =>
                s.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? s.Product.Images.First(i => i.IsPrimary).Url
                    : s.Product.Images.FirstOrDefault() != null ? s.Product.Images.First().Url : null))
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.Product.DiscountPrice ?? s.Product.Price))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Quantity * (s.Product.DiscountPrice ?? s.Product.Price)))
            .ForMember(d => d.VariantInfo, o => o.MapFrom(s =>
                s.ProductVariant != null ? $"{s.ProductVariant.Name}: {s.ProductVariant.Value}" : null));

        // Order
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"))
            .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.StatusLabel, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Address, o => o.MapFrom(s => s.Address));
        CreateMap<Address, AddressSnapshotDto>();
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Quantity * s.UnitPrice))
            .ForMember(d => d.VariantInfo, o => o.MapFrom(s =>
                s.ProductVariant != null ? $"{s.ProductVariant.Name}: {s.ProductVariant.Value}" : null));

        // Review
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"))
            .ForMember(d => d.UserImageUrl, o => o.MapFrom(s => s.User.ProfileImageUrl));

        // Coupon
        CreateMap<Coupon, CouponDto>();
        CreateMap<CreateCouponRequest, Coupon>();

        // Address
        CreateMap<Address, AddressDto>();
        CreateMap<CreateAddressRequest, Address>();
        CreateMap<UpdateAddressRequest, Address>();

        // Notification
        CreateMap<Notification, NotificationDto>();
    }
}
