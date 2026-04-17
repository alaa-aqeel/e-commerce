using ECommerceApi.Application.DTOs.Product;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/products")]
public class ProductsController : BaseController
{
    private readonly IProductService _products;

    public ProductsController(IProductService products) => _products = products;

    /// <summary>Get products with filtering, search, and pagination</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterRequest filter) =>
        HandleResult(await _products.GetAllAsync(filter));

    /// <summary>Get product details</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        HandleResult(await _products.GetByIdAsync(id));

    /// <summary>Create a product — Admin only</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request) =>
        HandleResult(await _products.CreateAsync(request));

    /// <summary>Update a product — Admin only</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request) =>
        HandleResult(await _products.UpdateAsync(id, request));

    /// <summary>Delete a product — Admin only</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        HandleResult(await _products.DeleteAsync(id));

    /// <summary>Upload product images — Admin only</summary>
    [HttpPost("{id}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImages(int id, [FromBody] List<string> imageUrls) =>
        HandleResult(await _products.AddImagesAsync(id, imageUrls));
}
