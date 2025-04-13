using System.Diagnostics;
using Drinkbox.Models;
using Drinkbox.Services.Brands;
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

        public HomeController(ILogger<HomeController> logger, IProductService productService, IBrandService brandService)
        {
            _logger = logger;
            _productService = productService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                var brands = await _brandService.GetAllBrandsAsync();

                var model = new ProductsViewModel
                {
                    Products = products,
                    Brands = new SelectList(brands, "BrandId", "BrandName"),
                    SelectedBrandID = 0
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
            var products = await _productService.GetProductsByBrandAsync(brandId);
            if (maxPrice.HasValue)
                products = _productService.GetProductsByMaxPrice(products, maxPrice.Value);

            return PartialView("_ProductListPartial", products);
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetPriceValues(int? brandId)
        {
            var products = await _productService.GetProductsByBrandAsync(brandId);

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
