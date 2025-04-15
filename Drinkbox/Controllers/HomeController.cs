using System.Diagnostics;
using Drinkbox.Models;
using Drinkbox.Services.Brands;
using Drinkbox.Services.CartItems;
using Drinkbox.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Drinkbox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICartItemService _cartItemService;

        public HomeController(ILogger<HomeController> logger, IProductService productService,
            IBrandService brandService, ICartItemService cartItemService)
        {
            _logger = logger;
            _productService = productService;
            _brandService = brandService;
            _cartItemService = cartItemService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                var brands = await _brandService.GetAllBrandsAsync();

                var model = new ProductsViewModel
                {
                    Products = products,
                    Brands = new SelectList(brands, "BrandId", "BrandName"),
                    ProductsInCart = new HashSet<int>(_cartItemService.CartItems.Select(x => x.ProductId))
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                return View(new ProductsViewModel
                {
                    Products = Enumerable.Empty<Product>(),
                    Brands = new SelectList(Enumerable.Empty<Brand>(), "BrandID", "BrandName")
                });
            }
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> FilterProducts(int? brandId, int? maxPrice)
        {
            var products = await _productService.GetByBrandAsync(brandId);
            if (maxPrice.HasValue)
                products = _productService.GetByMaxPrice(products, maxPrice.Value);

            var model = new ProductsViewModel
            {
                Products = products,
                ProductsInCart = new HashSet<int>(_cartItemService.CartItems.Select(x => x.ProductId))
            };

            return PartialView("_ProductListPartial", model);
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetPriceValues(int? brandId)
        {
            var products = await _productService.GetByBrandAsync(brandId);

            var maxPrice = products.Any() ? products.Max(p => p.Price) : 0;
            var minPrice = products.Any() ? products.Min(p => p.Price) : 0;
            return Json(new { minPrice, maxPrice });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
