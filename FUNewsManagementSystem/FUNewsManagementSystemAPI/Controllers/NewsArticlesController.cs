using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Identity.Client;
using Services;
using System.Security.Claims;

namespace FUNewsManagementSystemAPI.Controllers
{
    [Route("odata/NewsArticles")]
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // Read - Staff có thể xem tất cả news articles
        [Authorize(Policy = "StaffOnly")]
        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var articles = _newsArticleService.GetNewsArticles();
            return Ok(articles);
        }

        // Read single article - Staff có thể xem chi tiết
        [Authorize(Policy = "StaffOnly")]
        [EnableQuery]
        [HttpGet("{key}")]
        public IActionResult Get([FromODataUri] string key)
        {
            var article = _newsArticleService.GetNewsArticleById(key);
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        // Create - Staff có thể tạo news article
        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public IActionResult Post([FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Generate ID if not provided
            if (string.IsNullOrEmpty(article.NewsArticleId))
            {
                article.NewsArticleId = Guid.NewGuid().ToString();
            }

            // Set creator ID from token
            var creatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (short.TryParse(creatorId, out short accountId))
            {
                article.CreatedById = accountId;
            }

            article.CreatedDate = DateTime.UtcNow;
            _newsArticleService.AddNewsArticle(article);
            return Created(article);
        }

        // Update - Staff có thể cập nhật news article
        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{key}")]
        public IActionResult Put([FromODataUri] string key, [FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _newsArticleService.GetNewsArticleById(key);
            if (existing == null)
                return NotFound();

            article.NewsArticleId = key;
            article.ModifiedDate = DateTime.UtcNow;

            // Preserve original creator and created date
            article.CreatedById = existing.CreatedById;
            article.CreatedDate = existing.CreatedDate;

            _newsArticleService.UpdateNewsArticle(article);
            return Updated(article);
        }

        // Partial Update - Staff có thể cập nhật một phần
        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{key}")]
        public IActionResult Patch([FromODataUri] string key, [FromBody] Delta<NewsArticle> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _newsArticleService.GetNewsArticleById(key);
            if (existing == null)
                return NotFound();

            delta.Put(existing);
            existing.ModifiedDate = DateTime.UtcNow;
            _newsArticleService.UpdateNewsArticle(existing);
            return Updated(existing);
        }

        // Delete - Staff có thể xóa news article
        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{key}")]
        public IActionResult Delete([FromODataUri] string key)
        {
            var article = _newsArticleService.GetNewsArticleById(key);
            if (article == null)
                return NotFound();

            _newsArticleService.DeleteNewsArticle(article);
            return NoContent();
        }

        // Get news articles created by current staff member
        [Authorize(Policy = "StaffOnly")]
        [EnableQuery]
        [HttpGet("my-articles")]
        public IActionResult GetMyArticles()
        {
            var creatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!short.TryParse(creatorId, out short accountId))
            {
                return BadRequest("Invalid account ID");
            }

            var articles = _newsArticleService.GetNewsByCreator(accountId);
            return Ok(articles);
        }
    }
}