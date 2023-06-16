using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Contractors;
using Store.Data.Ef;
using Store.Messages;
using Store.Web.App;
using Store.Web.Contractors;
using Store.YandexKassa;
using System;


namespace Store.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Встроенный депендеси инжекшен (настраивает зависимости)
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //Обращение к HttpContext
            services.AddHttpContextAccessor();
            //Распределенная память для хранения сессии
            services.AddDirectoryBrowser();
            services.AddSession(options => 
            {
                //Параметры сессии как она будет храниться в куках
                //в данном случае 20 мин(время жизни сессии)
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                //Настройка параметров куков
                //не будет доступа из локальных js (только с сервера)
                options.Cookie.HttpOnly = true;
                //Получение согласия на сохранение данных в куках
                options.Cookie.IsEssential = true;
            });

            services.AddEfRepositories(Configuration.GetConnectionString("DefaultConnection"));
            services.AddSingleton<INotificationService, DebugNotificationService>();
            services.AddSingleton<IDeliveryService, PostamateDeliveryService>();
            services.AddSingleton<IPaymentService, CashPaymentService>();
            services.AddSingleton<IPaymentService, YandexKassaPaymentService>();
            services.AddSingleton<IWebContractorService, YandexKassaPaymentService>();
            services.AddSingleton<BookService>();
            services.AddSingleton<OrderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();//подключение сессий

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
               
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");               
            });
        }
    }
}
