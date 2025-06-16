using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;
using Services.DTO;

namespace FUNewsManagementSystemAPI.Controllers
{
    [Route("odata/Categories")]
    public class CategoriesController : ODataController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var categories = _categoryService.GetCategories();
            return Ok(categories);
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [EnableQuery]
        [HttpGet("{key}")]
        public IActionResult Get([FromODataUri] short key)
        {
            var category = _categoryService.GetCategoryById(key);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _categoryService.CreateCategory(category);
            return Created(category);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{key}")]
        public IActionResult Put([FromODataUri] short key, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _categoryService.GetCategoryById(key);
            if (existing == null)
                return NotFound();

            category.CategoryId = key;
            _categoryService.UpdateCategory(category);
            return Updated(category);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{key}")]
        public IActionResult Patch([FromODataUri] short key, [FromBody] Delta<Category> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _categoryService.GetCategoryById(key);
            if (existing == null)
                return NotFound();

            delta.Put(existing);
            _categoryService.UpdateCategory(existing);
            return Updated(existing);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{key}")]
        public IActionResult Delete([FromODataUri] short key)
        {
            var category = _categoryService.GetCategoryById(key);
            if (category == null)
                return NotFound();

            if (_categoryService.IsCategoryUsedInNews(key))
                return BadRequest("Cannot delete category used in news articles.");

            _categoryService.DeleteCategory(key);
            return NoContent();
        }
    }
}
