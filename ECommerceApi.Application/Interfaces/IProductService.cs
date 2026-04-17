using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Product;

namespace ECommerceApi.Application.Interfaces;

public interface IProductService
{
    Task<ServiceResult<PagedResult<ProductDto>>> GetAllAsync(ProductFilterRequest filter);
    Task<ServiceResult<ProductDto>> GetByIdAsync(int id);
    Task<ServiceResult<ProductDto>> CreateAsync(CreateProductRequest request);
    Task<ServiceResult<ProductDto>> UpdateAsync(int id, UpdateProductRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<List<ProductImageDto>>> AddImagesAsync(int productId, List<string> imageUrls);
}
