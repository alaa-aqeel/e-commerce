using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Product;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private IQueryable<Product> ProductQuery() =>
        _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Reviews);

    public async Task<ServiceResult<PagedResult<ProductDto>>> GetAllAsync(ProductFilterRequest filter)
    {
        var query = ProductQuery().AsNoTracking().Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Name.ToLower().Contains(filter.Search.ToLower()) ||
                                     p.Description.ToLower().Contains(filter.Search.ToLower()));

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) <= filter.MaxPrice.Value);

        query = filter.SortBy switch
        {
            "price_asc"  => query.OrderBy(p => p.DiscountPrice ?? p.Price),
            "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
            "newest"     => query.OrderByDescending(p => p.CreatedAt),
            _            => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return ServiceResult<PagedResult<ProductDto>>.Ok(new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(items),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    public async Task<ServiceResult<ProductDto>> GetByIdAsync(int id)
    {
        var product = await ProductQuery().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return ServiceResult<ProductDto>.Fail("Product not found.");
        return ServiceResult<ProductDto>.Ok(_mapper.Map<ProductDto>(product));
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(CreateProductRequest request)
    {
        if (await _context.Products.AnyAsync(p => p.SKU == request.SKU))
            return ServiceResult<ProductDto>.Fail("SKU already exists.");

        var product = _mapper.Map<Product>(request);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(product.Id);
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(int id, UpdateProductRequest request)
    {
        if (await _context.Products.AnyAsync(p => p.SKU == request.SKU && p.Id != id))
            return ServiceResult<ProductDto>.Fail("SKU already used by another product.");

        var product = await _context.Products.FindAsync(id);
        if (product == null) return ServiceResult<ProductDto>.Fail("Product not found.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.SKU = request.SKU;
        product.Price = request.Price;
        product.DiscountPrice = request.DiscountPrice;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return ServiceResult<bool>.Fail("Product not found.");
        product.IsDeleted = true;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<List<ProductImageDto>>> AddImagesAsync(int productId, List<string> imageUrls)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return ServiceResult<List<ProductImageDto>>.Fail("Product not found.");

        var hasPrimary = await _context.ProductImages.AnyAsync(i => i.ProductId == productId && i.IsPrimary);
        var images = imageUrls.Select((url, idx) => new ProductImage
        {
            ProductId = productId,
            Url = url,
            IsPrimary = !hasPrimary && idx == 0
        }).ToList();

        _context.ProductImages.AddRange(images);
        await _context.SaveChangesAsync();
        return ServiceResult<List<ProductImageDto>>.Ok(_mapper.Map<List<ProductImageDto>>(images));
    }
}
