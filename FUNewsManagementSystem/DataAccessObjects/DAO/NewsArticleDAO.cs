
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

            // Lấy tất cả ID từ database và chuyển sang danh sách
            var allIds = await context.NewsArticles
                                      .Select(n => n.NewsArticleId)
                                      .ToListAsync();

            // Tìm ID số lớn nhất trong danh sách (bỏ qua ID không phải số)
            int maxId = allIds
                        .Where(id => int.TryParse(id, out _)) // Lọc ID hợp lệ
                        .Select(int.Parse) // Chuyển sang số
                        .DefaultIfEmpty(0) // Nếu không có bản ghi nào, mặc định là 0
                        .Max(); // Lấy giá trị lớn nhất

            // Gán ID mới lớn hơn 1 đơn vị
            newsArticle.NewsArticleId = (maxId + 1).ToString();

            await context.NewsArticles.AddAsync(newsArticle);
            await context.SaveChangesAsync();
        }





        public static void SaveNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FunewsManagementContext())
            {
                context.NewsArticles.Add(newsArticle);
                context.SaveChanges();
            }
        }

        public static void UpdateNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FunewsManagementContext())
            {
                context.NewsArticles.Update(newsArticle);
                context.SaveChanges();
            }
        }

        public static void DeleteNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FunewsManagementContext())
            {
                context.NewsArticles.Remove(newsArticle);
                context.SaveChanges();
            }
        }

        public static NewsArticle GetNewsArticleById(string id)
        {
            using var db = new FunewsManagementContext();
            var article = db.NewsArticles.FirstOrDefault(n => n.NewsArticleId == id);

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
                          .ToList();
        }
        public static List<NewsArticle> SearchNewsByKeyword(string keyword)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                          .Where(n => n.NewsTitle.Contains(keyword) || n.NewsContent.Contains(keyword))
                          .Include(n => n.Category)
                          .Include(n => n.NewsTags)
                          .ToList();
        }
        public static List<NewsArticle> GetActiveNewsArticles()
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                          .Where(n => n.NewsStatus == true)
                          .Include(n => n.Category)
                          .Include(n => n.NewsTags)
                          .ToList();
        }

    }
}