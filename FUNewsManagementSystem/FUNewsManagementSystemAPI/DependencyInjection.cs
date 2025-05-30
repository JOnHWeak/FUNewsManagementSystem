using Repositories;
using Services;

namespace FUNewsManagementSystemAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các dịch vụ
            services.AddScoped<ISystemAccountService, SystemAccountService>();
            services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
            services.AddScoped<INewsArticleService, NewsArticleService>();
            services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();


            return services;
        }
    }
}
