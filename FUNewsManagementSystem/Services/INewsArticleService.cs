using BusinessObjects;

using System.Collections.Generic;

namespace Services
{
    public interface INewsArticleService
    {
        void AddNewsArticle(NewsArticle newsArticle);
        void SaveNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        bool NewsArticleExists(string id);
        List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate);
        List<NewsArticle> GetNewsByCreator(short creatorId);
        List<NewsArticle> SearchNewsByKeyword(string keyword);

    }
}
