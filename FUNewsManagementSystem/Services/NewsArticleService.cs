using BusinessObjects;
using DataAccessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository newsArticleRepository;

        public NewsArticleService()
        {
            newsArticleRepository = new NewsArticleRepository();
        }

        public void AddNewsArticle(NewsArticle newNewsArticle)
        {
            newsArticleRepository.AddNewsArticle(newNewsArticle); 
        }

        public void SaveNewsArticle(NewsArticle newsArticle)
        {
            newsArticleRepository.SaveNewsArticle(newsArticle);
        }

        public void DeleteNewsArticle(NewsArticle newsArticle)
        {
            newsArticleRepository.DeleteNewsArticle(newsArticle);
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            newsArticleRepository.UpdateNewsArticle(newsArticle);
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return newsArticleRepository.GetNewsArticles();
        }

        public NewsArticle GetNewsArticleById(string id)
        {
            return newsArticleRepository.GetNewsArticleById(id);
        }

        public bool NewsArticleExists(string id)
        {
            return newsArticleRepository.GetNewsArticleById(id) != null; 
        }
        public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
        {
            return newsArticleRepository.GetNewsArticlesByPeriod(startDate, endDate);
        }
        public List<NewsArticle> GetNewsArticlesByDateRange(DateTime startDate, DateTime endDate)
        {
            using var context = new FunewsManagementContext();

            return context.NewsArticles
                          .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                          .OrderByDescending(n => n.CreatedDate) // Sắp xếp giảm dần
                          .ToList();
        }
        public List<NewsArticle> GetNewsByCreator(short creatorId)
        {
            return newsArticleRepository.GetNewsByCreator(creatorId);
        }

        public List<NewsArticle> SearchNewsByKeyword(string keyword)
        {
            return newsArticleRepository.SearchNewsByKeyword(keyword);
        }

    }
}
