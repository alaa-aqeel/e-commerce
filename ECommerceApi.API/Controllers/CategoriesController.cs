using ECommerceApi.Application.DTOs.Category;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/categories")]
public class CategoriesController : BaseController
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories) => _categories = categories;

    /// <summary>Get all root categories with sub-categories</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        HandleResult(await _categories.GetAllAsync());

    /// <summary>Get category by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        HandleResult(await _categories.GetByIdAsync(id));

    /// <summary>Create a category — Admin only</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request) =>
        HandleResult(await _categories.CreateAsync(request));

    /// <summary>Update a category — Admin only</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request) =>
        HandleResult(await _categories.UpdateAsync(id, request));

    /// <summary>Delete a category — Admin only</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        HandleResult(await _categories.DeleteAsync(id));
}
