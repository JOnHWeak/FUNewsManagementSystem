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
    [Route("odata/NewsArticles")]
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var articles = _newsArticleService.GetNewsArticles();
            return Ok(articles);
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [EnableQuery]
        [HttpGet("{key}")]
        public IActionResult Get([FromODataUri] string key)
        {
            var article = _newsArticleService.GetNewsArticleById(key);
            if (article == null)
                return NotFound();

            return Ok(article);
        }

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

            article.CreatedDate = DateTime.UtcNow;
            _newsArticleService.AddNewsArticle(article);
            return Created(article);
        }

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
            _newsArticleService.UpdateNewsArticle(article);
            return Updated(article);
        }

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

        // Custom actions using OData conventions
        [Authorize(Policy = "StaffOnly")]
        [EnableQuery]
        [HttpGet("odata/NewsArticles/GetByCreator(accountId={accountId})")]
        public IActionResult GetByCreator([FromODataUri] short accountId)
        {
            var articles = _newsArticleService.GetNewsByCreator(accountId);
            return Ok(articles);
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [EnableQuery]
        [HttpGet("odata/NewsArticles/SearchByKeyword(keyword='{keyword}')")]
        public IActionResult SearchByKeyword([FromODataUri] string keyword)
        {
            var articles = _newsArticleService.SearchNewsByKeyword(keyword);
            return Ok(articles);
        }
    }
}
