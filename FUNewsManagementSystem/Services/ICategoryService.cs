
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        List<Category> GetCategories();
        bool IsCategoryUsedInNews(short categoryId);
        void DeleteCategory(short categoryId);
        void CreateCategory(Category category);
        void UpdateCategory(Category category);


    }
}
