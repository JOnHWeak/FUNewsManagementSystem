using BusinessObjects;
using Services.DTO.Services.DTO;
using System.Collections.Generic;

namespace Services
{
    public interface INewsArticleService
    {
        // Existing methods from your code
        void AddNewsArticle(NewsArticle newNewsArticle);
        void SaveNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        bool NewsArticleExists(string id);
        List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate);
        List<NewsArticle> GetNewsByCreator(short creatorId);
        List<NewsArticle> SearchNewsByKeyword(string keyword);
        List<NewsArticle> GetActiveNewsArticles();

        // Enhanced methods
        Task<NewsArticleResponseDto> AddNewsArticleAsync(NewsArticleRequestDto request);
        Task<NewsArticleResponseDto> UpdateNewsArticleAsync(string id, NewsArticleRequestDto request);
        Task<bool> DeleteNewsArticleAsync(string id);
        NewsArticleResponseDto GetNewsArticleResponseDto(string id);
        IQueryable<NewsArticle> GetNewsArticlesQueryable();

    }
}
