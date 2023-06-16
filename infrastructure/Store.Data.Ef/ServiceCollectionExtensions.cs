using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data.Ef
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<StoreDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                },
                ServiceLifetime.Transient
            );
            //Словарь будет создаваться на каждый запрос пользователя
            services.AddScoped<Dictionary<Type, StoreDbContext>>();
            //DbContext будет создаваться один на каждый репозиторий и
            //на каждый запрос
            services.AddSingleton<DbContextFactory>();
            //Репозитории будут создаваться и храниться по одному экземпляру
            //на всю работу приложения 
            services.AddSingleton<IBookRepository, BookRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
