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
                var products = await _productService.GetAllProductsAsyc();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
