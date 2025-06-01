using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;
using Services.DTO.Services.DTO;
using System.Linq;

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
            var articles = _newsArticleService.GetNewsArticles();
            var dtoList = articles.Select(ToResponseDto).ToList();
            return Ok(dtoList);
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var article = _newsArticleService.GetNewsArticleById(id);
            if (article == null) return NotFound();

            return Ok(ToResponseDto(article));
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public IActionResult Create([FromBody] NewsArticleRequestDto dto)
        {
            var article = ToEntity(dto);
            _newsArticleService.AddNewsArticle(article);
            return Ok();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] NewsArticleRequestDto dto)
        {
            // Since NewsArticleRequestDto does not have ID, just check id presence.
            var existing = _newsArticleService.GetNewsArticleById(id);
            if (existing == null) return NotFound();

            var article = ToEntity(dto);
            article.NewsArticleId = id; // ensure ID is set
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
            var list = _newsArticleService.GetNewsByCreator(accountId);
            var dtoList = list.Select(ToResponseDto).ToList();
            return Ok(dtoList);
        }

        [Authorize(Policy = "AdminOrStaffOrLecturer")]
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string keyword)
        {
            var list = _newsArticleService.SearchNewsByKeyword(keyword);
            var dtoList = list.Select(ToResponseDto).ToList();
            return Ok(dtoList);
        }

        private static NewsArticleResponseDto ToResponseDto(NewsArticle article)
        {
            return new NewsArticleResponseDto
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.CategoryName,
                NewsStatus = article.NewsStatus,
                CreatedById = article.CreatedById,
                CreatedByName = article.CreatedBy?.AccountName,
                UpdatedById = article.UpdatedById,
                ModifiedDate = article.ModifiedDate
                // Add Tags mapping here if needed
            };
        }

        private static NewsArticle ToEntity(NewsArticleRequestDto dto)
        {
            return new NewsArticle
            {
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId,
                NewsStatus = dto.NewsStatus,
                CreatedById = dto.CreatedById,
                UpdatedById = dto.UpdatedById
                // Handle NewsTags if needed
            };
        }
    }
}
