using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Category;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();

        return ServiceResult<List<CategoryDto>>.Ok(_mapper.Map<List<CategoryDto>>(categories));
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return ServiceResult<CategoryDto>.Fail("Category not found.");
        return ServiceResult<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }

    public async Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(category.Id);
    }

    public async Task<ServiceResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return ServiceResult<CategoryDto>.Fail("Category not found.");

        _mapper.Map(request, category);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return ServiceResult<bool>.Fail("Category not found.");
        category.IsDeleted = true;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
