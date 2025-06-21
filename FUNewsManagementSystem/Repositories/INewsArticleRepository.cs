using BusinessObjects;
using System;
using System.Collections.Generic;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        void DeleteNewsArticle(NewsArticle newsArticle);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        void SaveNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        void AddNewsArticle(NewsArticle newsArticle);
        List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate);
        List<NewsArticle> GetNewsByCreator(short creatorId);
        List<NewsArticle> SearchNewsByKeyword(string keyword);
        List<NewsArticle> GetActiveNewsArticles();

        // Additional methods for OData
        Task AddNewsArticleAsync(NewsArticle newsArticle);
        Task<NewsArticle> UpdateNewsArticleAsync(NewsArticle newsArticle);
        IQueryable<NewsArticle> GetNewsArticlesQueryable();

    }
}
