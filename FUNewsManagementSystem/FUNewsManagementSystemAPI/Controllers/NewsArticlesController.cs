using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace FUNewsManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }
        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [HttpGet]
        public IActionResult GetAllNews()
        {
            return Ok(_newsArticleService.GetNewsArticles());
        }
        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var article = _newsArticleService.GetNewsArticleById(id);
            if (article == null) return NotFound();
            return Ok(article);
        }
        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public IActionResult Create([FromBody] NewsArticle article)
        {
            _newsArticleService.AddNewsArticle(article);
            return Ok();
        }
        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] NewsArticle article)
        {
            if (id != article.NewsArticleId) return BadRequest();
            _newsArticleService.UpdateNewsArticle(article);
            return Ok();
        }
        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var article = _newsArticleService.GetNewsArticleById(id);
            if (article == null) return NotFound();
            _newsArticleService.DeleteNewsArticle(article);
            return Ok();
        }
        [Authorize(Policy = "StaffOnly")]
        [HttpGet("by-creator/{accountId}")]
        public IActionResult GetByCreator(short accountId)
        {
            var list = _newsArticleService.GetNewsByCreator(accountId); // NEW method in service
            return Ok(list);
        }
        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string keyword)
        {
            var list = _newsArticleService.SearchNewsByKeyword(keyword); // NEW method in service
            return Ok(list);
        }
    }
}
