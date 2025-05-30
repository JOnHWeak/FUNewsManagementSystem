using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class NewsTagDAO
    {
        public static void AddNewsTag(string newsArticleId, int tagId)
        {
            using var context = new FunewsManagementContext();
            var newsTag = new NewsTag { NewsArticleId = newsArticleId, TagId = tagId };
            context.NewsTags.Add(newsTag);
            context.SaveChanges();
        }

        public static void RemoveNewsTag(string newsArticleId, int tagId)
        {
            using var context = new FunewsManagementContext();
            var newsTag = context.NewsTags.FirstOrDefault(nt => nt.NewsArticleId == newsArticleId && nt.TagId == tagId);
            if (newsTag != null)
            {
                context.NewsTags.Remove(newsTag);
                context.SaveChanges();
            }
        }

        public static List<Tag> GetTagsByNewsArticle(string newsArticleId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsTags
                          .Where(nt => nt.NewsArticleId == newsArticleId)
                          .Include(nt => nt.Tag)
                          .Select(nt => nt.Tag)
                          .ToList();
        }
    }

}
