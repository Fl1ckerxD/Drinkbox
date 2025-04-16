using Drinkbox.Models;
using Drinkbox.Services.Brands;
using Drinkbox.Services.CartItems;
using Drinkbox.Services.Coins;
using Drinkbox.Services.Products;
using Microsoft.EntityFrameworkCore;

namespace Drinkbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            var conString = builder.Configuration.GetConnectionString("DrinkBoxDatabase") ??
                throw new InvalidOperationException("Connection string 'DrinkBoxDatabase' not found.");
            builder.Services.AddDbContext<DrinkboxContext>(options => options.UseSqlServer(conString));

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<ICoinService, CoinService>();

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
