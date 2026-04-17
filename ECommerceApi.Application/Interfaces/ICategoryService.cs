using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Category;

namespace ECommerceApi.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryDto>>> GetAllAsync();
    Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryRequest request);
    Task<ServiceResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
