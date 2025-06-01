using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        bool IsCategoryUsedInNews(short categoryId);
        void DeleteCategory(short categoryId);
        void AddCategory(Category category);
        void UpdateCategory(Category category);

        Category? GetCategoryById(short categoryId);

    }
}
