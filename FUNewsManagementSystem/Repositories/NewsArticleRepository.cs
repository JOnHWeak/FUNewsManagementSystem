using BusinessObjects;

using System.Collections.Generic;
using DataAccessObjects.DAO;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public void DeleteNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.DeleteNewsArticle(newsArticle);
        public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.GetNewsArticles();
        public NewsArticle GetNewsArticleById(string id) => NewsArticleDAO.GetNewsArticleById(id);
        public void SaveNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.SaveNewsArticle(newsArticle);
        public void UpdateNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.UpdateNewsArticle(newsArticle);
        public void AddNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.AddNewsArticle(newsArticle).Wait();
        public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate) => NewsArticleDAO.GetNewsArticlesByPeriod(startDate, endDate);
        public List<NewsArticle> GetNewsByCreator(short creatorId) => NewsArticleDAO.GetNewsByCreator(creatorId);
        public List<NewsArticle> SearchNewsByKeyword(string keyword) => NewsArticleDAO.SearchNewsByKeyword(keyword);
        public List<NewsArticle> GetActiveNewsArticles() => NewsArticleDAO.GetActiveNewsArticles();

        // Async methods
        public async Task AddNewsArticleAsync(NewsArticle newsArticle) => await NewsArticleDAO.AddNewsArticle(newsArticle);
        public async Task<NewsArticle> UpdateNewsArticleAsync(NewsArticle newsArticle) => await NewsArticleDAO.UpdateNewsArticleAsync(newsArticle);

        // OData
        public IQueryable<NewsArticle> GetNewsArticlesQueryable() => NewsArticleDAO.GetNewsArticlesQueryable();

    }
}
