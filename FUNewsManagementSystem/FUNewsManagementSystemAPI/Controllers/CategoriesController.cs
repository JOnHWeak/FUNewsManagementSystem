using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;
using BusinessObjects;

namespace FUNewsManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _categoryService.GetCategories();

            var result = categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive,
                ParentCategory = c.ParentCategory != null ? new ParentCategoryDto
                {
                    CategoryId = c.ParentCategory.CategoryId,
                    CategoryName = c.ParentCategory.CategoryName
                } : null,
                ChildCategories = c.InverseParentCategory.Select(child => new ChildCategoryDto
                {
                    CategoryId = child.CategoryId,
                    CategoryName = child.CategoryName
                }).ToList()
            });

            return Ok(result);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public IActionResult CreateCategory([FromBody] CategoryRequestDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDescription,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = dto.IsActive
            };

            _categoryService.CreateCategory(category);
            return Ok();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(short id, [FromBody] CategoryRequestDto dto)
        {
            var existing = _categoryService.GetCategoryById(id);
            if (existing == null) return NotFound();

            existing.CategoryName = dto.CategoryName;
            existing.CategoryDesciption = dto.CategoryDescription;
            existing.ParentCategoryId = dto.ParentCategoryId;
            existing.IsActive = dto.IsActive;

            _categoryService.UpdateCategory(existing);
            return Ok();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(short id)
        {
            if (_categoryService.IsCategoryUsedInNews(id))
                return BadRequest("Cannot delete category used in news articles.");

            _categoryService.DeleteCategory(id);
            return Ok("Category deleted");
        }
    }
}
