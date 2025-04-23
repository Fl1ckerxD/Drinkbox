using Drinkbox.Infrastructure.Data;
using Drinkbox.Infrastructure.Repositories;
using Drinkbox.Infrastructure.Services.Brands;
using Drinkbox.Infrastructure.Services.CartItems;
using Drinkbox.Infrastructure.Services.Coins;
using Drinkbox.Infrastructure.Services.ExcelImports;
using Drinkbox.Infrastructure.Services.Products;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews().AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/Web/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                options.ViewLocationFormats.Add("/Web/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddResponseCompression(opt =>
            {
                opt.EnableForHttps = true;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            var conString = builder.Configuration.GetConnectionString("VendomatDatabase") ??
                throw new InvalidOperationException("Connection string 'VendomatDatabase' not found.");
            builder.Services.AddDbContext<VendomatContext>(options => options.UseSqlServer(conString));

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<ICoinService, CoinService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddTransient<IExcelImportService, ExcelImportService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseResponseCompression();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
