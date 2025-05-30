using BusinessObjects;
using System;
using System.Collections.Generic;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        void AddNewsArticle(NewsArticle newsArticle);
        void SaveNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate);
        List<NewsArticle> GetNewsByCreator(short creatorId);
        List<NewsArticle> SearchNewsByKeyword(string keyword);

    }
}
