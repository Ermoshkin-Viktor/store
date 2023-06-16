using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data.Ef
{
    //Благодаря этому классу репозитории могут быть Singleton а 
    //DbContext на каждый запрос свой
    public class DbContextFactory
    {
        //Делает обращение к DbContext косвенным
        //Не получаем напрямую. Каждый DbContext запрашивает у WebRequest
        //HttpContextAccessor предоставляет возможность обратиться к HttpContext
        //и к Scope в котором храняться все объекты текущего запроса
        private readonly IHttpContextAccessor httpContextAccessor;

        public DbContextFactory(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ;
        }
        //BookRepository будет иметь свой контекст, а OrderRepository-свой
        public StoreDbContext Create(Type repositoryType)
        {
            //Мы обращаемся к контейнеру который пришел с последним запросом
            var services = httpContextAccessor.HttpContext.RequestServices;
            //Мы там храним словарь в который добавляем DbContext
            //если его еще не было ,а если он там есть то мы его берем
            //Type-ключ, StoreDbContext-значение
            var dbContexts = services.GetService<Dictionary<Type, StoreDbContext>>();
            if(!dbContexts.ContainsKey(repositoryType)) dbContexts[repositoryType] = 
                                             services.GetService<StoreDbContext>();

            return dbContexts[repositoryType];
        }
    }
}
