using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

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
            return Ok(categories);
        }
        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            _categoryService.CreateCategory(category);
            return Ok();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(short id, [FromBody] Category category)
        {
            if (id != category.CategoryId) return BadRequest();
            _categoryService.UpdateCategory(category);
            return Ok();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(short id)
        {
            bool isUsed = _categoryService.IsCategoryUsedInNews(id); // NEW method in service
            if (isUsed)
                return BadRequest("Cannot delete category used in news articles.");

            _categoryService.DeleteCategory(id); // Ensure service has this method
            return Ok("Category deleted");
        }

        // Add Create, Update, Search methods as needed
    }

}
