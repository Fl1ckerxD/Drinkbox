using System.Diagnostics;
using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Services.Brands;
using Drinkbox.Infrastructure.Services.CartItems;
using Drinkbox.Infrastructure.Services.ExcelImports;
using Drinkbox.Infrastructure.Services.Products;
using Drinkbox.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Drinkbox.Web.Controllers.Home
{
    /// <summary>
    /// Контроллер для управления главной страницей приложения.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICartItemService _cartItemService;
        private readonly IExcelImportService _excelImportService;

        public HomeController(ILogger<HomeController> logger, IProductService productService,
            IBrandService brandService, ICartItemService cartItemService, IExcelImportService excelImportService)
        {
            _logger = logger;
            _productService = productService;
            _brandService = brandService;
            _cartItemService = cartItemService;
            _excelImportService = excelImportService;
        }

        /// <summary>
        /// Возвращает главную страницу с информацией о продуктах, брендах и товарах в корзине.
        /// </summary>
        /// <returns>Представление главной страницы или пустое представление в случае ошибки.</returns>
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

        /// <summary>
        /// Фильтрует продукты по бренду или максимальной цене.
        /// </summary>
        /// <param name="brandId">Идентификатор бренда для фильтрации (необязательный).</param>
        /// <param name="maxPrice">Максимальная цена для фильтрации (необязательный).</param>
        /// <returns>Частичное представление с отфильтрованным списком продуктов.</returns>
        [HttpGet]
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

        /// <summary>
        /// Возвращает минимальную и максимальную цены продуктов для указанного бренда.
        /// </summary>
        /// <param name="brandId">Идентификатор бренда для получения цен (необязательный).</param>
        /// <returns>JSON с минимальной и максимальной ценами.</returns>
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetPriceValues(int? brandId)
        {
            var products = await _productService.GetByBrandAsync(brandId);

            var maxPrice = products.Any() ? products.Max(p => p.Price) : 0;
            var minPrice = products.Any() ? products.Min(p => p.Price) : 0;
            return Json(new { minPrice, maxPrice });
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportExcelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                await using var stream = model.ExcelFile.OpenReadStream();
                await _excelImportService.ImportProductsAsync(stream);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка импорта: {ex.Message}";
                _logger.LogError(ex, "Ошибка при импорте продуктов");
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
