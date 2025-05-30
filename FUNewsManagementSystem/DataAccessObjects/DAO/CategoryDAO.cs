
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class CategoryDAO
    {
        public static List<Category> GetCategories()
        {
            var listCategories = new List<Category>();
            try
            {
                using var context = new FunewsManagementContext();
                listCategories = context.Categories.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listCategories;
        }
        public static void AddCategory(Category category)
        {
            using var context = new FunewsManagementContext();
            context.Categories.Add(category);
            context.SaveChanges();
        }

        public static void UpdateCategory(Category category)
        {
            using var context = new FunewsManagementContext();
            var existing = context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existing == null)
                throw new Exception("Category not found.");

            existing.CategoryName = category.CategoryName;
            // thêm các trường khác nếu có
            context.SaveChanges();
        }

        public static bool IsCategoryUsedInNews(short categoryId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles.Any(n => n.CategoryId == categoryId);
        }

        public static void DeleteCategory(short categoryId)
        {
            using var context = new FunewsManagementContext();
            var category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
                throw new Exception($"Category with ID {categoryId} not found.");

            context.Categories.Remove(category);
            context.SaveChanges();
        }
    }
}
