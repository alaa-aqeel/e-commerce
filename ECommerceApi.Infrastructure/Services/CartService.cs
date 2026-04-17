using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Cart;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CartService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Images)
            .Include(c => c.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }
        return cart;
    }

    public async Task<ServiceResult<CartDto>> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return ServiceResult<CartDto>.Ok(_mapper.Map<CartDto>(cart));
    }

    public async Task<ServiceResult<CartDto>> AddItemAsync(int userId, AddToCartRequest request)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null) return ServiceResult<CartDto>.Fail("Product not found.");
        if (product.Stock < request.Quantity) return ServiceResult<CartDto>.Fail("Insufficient stock.");

        var cart = await GetOrCreateCartAsync(userId);
        var existing = cart.Items.FirstOrDefault(i =>
            i.ProductId == request.ProductId && i.ProductVariantId == request.ProductVariantId);

        if (existing != null)
            existing.Quantity += request.Quantity;
        else
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                ProductVariantId = request.ProductVariantId
            });

        await _context.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<ServiceResult<CartDto>> UpdateItemAsync(int userId, int itemId, UpdateCartItemRequest request)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return ServiceResult<CartDto>.Fail("Cart item not found.");

        if (request.Quantity <= 0)
            cart.Items.Remove(item);
        else
            item.Quantity = request.Quantity;

        await _context.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<ServiceResult<bool>> RemoveItemAsync(int userId, int itemId)
    {
        var item = await _context.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.UserId == userId);

        if (item == null) return ServiceResult<bool>.Fail("Cart item not found.");
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> ClearCartAsync(int userId)
    {
        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return ServiceResult<bool>.Ok(true);
        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
