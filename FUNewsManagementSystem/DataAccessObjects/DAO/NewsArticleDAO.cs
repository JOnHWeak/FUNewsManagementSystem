
using BusinessObjects;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class NewsArticleDAO
    {
        public static List<NewsArticle> GetNewsArticles()
        {
            using (var context = new FunewsManagementContext())
            {
                return context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                    .Include(n => n.CreatedBy)
                    .ToList();
            }
        }

        public static async Task AddNewsArticle(NewsArticle newsArticle)
        {
            using var context = new FunewsManagementContext();

            // Auto-generate ID
            var allIds = await context.NewsArticles
                                      .Select(n => n.NewsArticleId)
                                      .ToListAsync();

            int maxId = allIds
                        .Where(id => int.TryParse(id, out _))
                        .Select(int.Parse)
                        .DefaultIfEmpty(0)
                        .Max();

            newsArticle.NewsArticleId = (maxId + 1).ToString();
            newsArticle.CreatedDate = DateTime.UtcNow;

            // Handle tags properly
            if (newsArticle.NewsTags != null && newsArticle.NewsTags.Any())
            {
                foreach (var newsTag in newsArticle.NewsTags)
                {
                    newsTag.NewsArticleId = newsArticle.NewsArticleId;
                }
            }

            await context.NewsArticles.AddAsync(newsArticle);
            await context.SaveChangesAsync();
        }

        public static void SaveNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FunewsManagementContext())
            {
                if (newsArticle.CreatedDate == null)
                    newsArticle.CreatedDate = DateTime.UtcNow;

                context.NewsArticles.Add(newsArticle);
                context.SaveChanges();
            }
        }

        public static void UpdateNewsArticle(NewsArticle newsArticle)
        {
            using var context = new FunewsManagementContext();

            var existingArticle = context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefault(n => n.NewsArticleId == newsArticle.NewsArticleId);

            if (existingArticle == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID '{newsArticle.NewsArticleId}' not found.");
            }

            // Update properties
            existingArticle.NewsTitle = newsArticle.NewsTitle;
            existingArticle.Headline = newsArticle.Headline;
            existingArticle.NewsContent = newsArticle.NewsContent;
            existingArticle.NewsSource = newsArticle.NewsSource;
            existingArticle.CategoryId = newsArticle.CategoryId;
            existingArticle.NewsStatus = newsArticle.NewsStatus;
            existingArticle.UpdatedById = newsArticle.UpdatedById;
            existingArticle.ModifiedDate = DateTime.UtcNow;

            // Update tags
            if (newsArticle.NewsTags != null)
            {
                // Remove existing tags
                context.NewsTags.RemoveRange(existingArticle.NewsTags);

                // Add new tags
                foreach (var newsTag in newsArticle.NewsTags)
                {
                    newsTag.NewsArticleId = newsArticle.NewsArticleId;
                    context.NewsTags.Add(newsTag);
                }
            }

            context.SaveChanges();
        }

        public static async Task<NewsArticle> UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            using var context = new FunewsManagementContext();

            var existingArticle = await context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticle.NewsArticleId);

            if (existingArticle == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID '{newsArticle.NewsArticleId}' not found.");
            }

            // Update properties
            existingArticle.NewsTitle = newsArticle.NewsTitle;
            existingArticle.Headline = newsArticle.Headline;
            existingArticle.NewsContent = newsArticle.NewsContent;
            existingArticle.NewsSource = newsArticle.NewsSource;
            existingArticle.CategoryId = newsArticle.CategoryId;
            existingArticle.NewsStatus = newsArticle.NewsStatus;
            existingArticle.UpdatedById = newsArticle.UpdatedById;
            existingArticle.ModifiedDate = DateTime.UtcNow;

            // Update tags
            if (newsArticle.NewsTags != null)
            {
                context.NewsTags.RemoveRange(existingArticle.NewsTags);

                foreach (var newsTag in newsArticle.NewsTags)
                {
                    newsTag.NewsArticleId = newsArticle.NewsArticleId;
                    context.NewsTags.Add(newsTag);
                }
            }

            await context.SaveChangesAsync();

            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticle.NewsArticleId);
        }

        public static void DeleteNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FunewsManagementContext())
            {
                var articleToDelete = context.NewsArticles
                    .Include(n => n.NewsTags)
                    .FirstOrDefault(n => n.NewsArticleId == newsArticle.NewsArticleId);

                if (articleToDelete != null)
                {
                    // Remove tags first
                    context.NewsTags.RemoveRange(articleToDelete.NewsTags);
                    context.NewsArticles.Remove(articleToDelete);
                    context.SaveChanges();
                }
            }
        }

        public static NewsArticle GetNewsArticleById(string id)
        {
            using var db = new FunewsManagementContext();
            var article = db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Include(n => n.CreatedBy)
                .FirstOrDefault(n => n.NewsArticleId == id);

            if (article == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID '{id}' not found.");
            }

            return article;
        }

        public static List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }

        public static List<NewsArticle> GetNewsByCreator(short creatorId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                          .Where(n => n.CreatedById == creatorId)
                          .Include(n => n.Category)
                          .Include(n => n.NewsTags)
                              .ThenInclude(nt => nt.Tag)
                          .Include(n => n.CreatedBy)
                          .ToList();
        }

        public static List<NewsArticle> SearchNewsByKeyword(string keyword)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                          .Where(n => n.NewsTitle.Contains(keyword) ||
                                     n.NewsContent.Contains(keyword) ||
                                     n.NewsTags.Any(nt => nt.Tag.TagName.Contains(keyword)))
                          .Include(n => n.Category)
                          .Include(n => n.NewsTags)
                              .ThenInclude(nt => nt.Tag)
                          .Include(n => n.CreatedBy)
                          .ToList();
        }

        public static List<NewsArticle> GetActiveNewsArticles()
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                          .Where(n => n.NewsStatus == true)
                          .Include(n => n.Category)
                          .Include(n => n.NewsTags)
                              .ThenInclude(nt => nt.Tag)
                          .Include(n => n.CreatedBy)
                          .ToList();
        }

        public static IQueryable<NewsArticle> GetNewsArticlesQueryable()
        {
            var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Include(n => n.CreatedBy)
                .AsQueryable();
        }
    }
}
