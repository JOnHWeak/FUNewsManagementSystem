
using BusinessObjects;
using DataAccessObjects.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetCategories() => CategoryDAO.GetCategories();
        public bool IsCategoryUsedInNews(short categoryId) =>
    CategoryDAO.IsCategoryUsedInNews(categoryId);

        public void DeleteCategory(short categoryId) =>
            CategoryDAO.DeleteCategory(categoryId);
        public void AddCategory(Category category) => CategoryDAO.AddCategory(category);
        public void UpdateCategory(Category category) => CategoryDAO.UpdateCategory(category);

    }
}
