using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService()
        {
            categoryRepository = new CategoryRepository();
        }

        public List<Category> GetCategories()
        {
            return categoryRepository.GetCategories();
        }
        public bool IsCategoryUsedInNews(short categoryId)
        {
            return categoryRepository.IsCategoryUsedInNews(categoryId);
        }

        public void DeleteCategory(short categoryId)
        {
            categoryRepository.DeleteCategory(categoryId);
        }
        public void CreateCategory(Category category)
        {
            categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            categoryRepository.UpdateCategory(category);
        }

        public Category? GetCategoryById(short categoryId)
        {
            return categoryRepository.GetCategoryById(categoryId);
        }


    }
}
