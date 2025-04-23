using Drinkbox.Infrastructure.Services.CartItems;
using Drinkbox.Infrastructure.Services.Products;
using Drinkbox.Web.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Drinkbox.Web.Controllers.Cart
{
    /// <summary>
    /// Контроллер для управления корзиной покупок
    /// </summary>
    public class CartController : Controller
    {
        private readonly ICartItemService _cartItemService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;
        public CartController(ILogger<CartController> logger, ICartItemService cartItemService,
            IProductService productService)
        {
            _logger = logger;
            _cartItemService = cartItemService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            // Если в корзине нет товаров, перенаправляем пользователя на главную страницу
            if (_cartItemService.CartItems.Count == 0)
                return RedirectToAction("Index", "Home");

            var cartItems = _cartItemService.CartItems;
            return View(cartItems);
        }

        /// <summary>
        /// Удаляет товар из корзины по его ID.
        /// </summary>
        /// <param name="productId">Идентификатор товара для удаления.</param>
        /// <returns>HTTP 200 (OK), если товар успешно удален, или HTTP 404 (Not Found), если товар не найден.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var product = await _productService.GetByIdAsync(productId);

                _cartItemService.RemoveFromCart(product);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления продкута из корзины");
                return NotFound();
            }
        }

        /// <summary>
        /// Возвращает статус корзины: количество товаров и наличие товаров.
        /// </summary>
        /// <returns>JSON с количеством товаров и флагом наличия товаров.</returns>
        [HttpGet]
        public IActionResult GetCartStatus()
        {
            var cartItems = _cartItemService.CartItems;
            return Json(new { itemCount = cartItems.Count, hasItems = cartItems.Any() });
        }

        /// <summary>
        /// Добавляет или удаляет товар из корзины.
        /// </summary>
        /// <param name="request">Запрос, содержащий ID товара и флаг isSelected.</param>
        /// <returns>HTTP 200 (OK), если операция выполнена успешно, или HTTP 404 (Not Found), если товар не найден.</returns>
        [HttpPost]
        public async Task<IActionResult> ToggleProduct([FromBody] ToggleProductRequest request)
        {
            try
            {
                var product = await _productService.GetByIdAsync(request.productId);

                if (request.isSelected)
                    _cartItemService.AddToCart(product);
                else
                    _cartItemService.RemoveFromCart(product);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления/добавления продукта в корзину");
                return NotFound();
            }
        }


        /// <summary>
        /// Обновляет количество товара в корзине.
        /// </summary>
        /// <param name="request">Запрос, содержащий ID товара и новое количество.</param>
        /// <returns>JSON с результатом операции, фактическим количеством и максимальным количеством товара.</returns>
        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var product = _cartItemService.CartItems.FirstOrDefault(x => x.ProductId == request.productId);

            if (product != null)
            {
                var quantity = request.quantity;
                quantity = Math.Min(quantity, product.MaxQuantity);
                quantity = Math.Max(1, quantity);

                product.Quantity = quantity;
                _cartItemService.SaveCart();

                return Json(new
                {
                    success = true,
                    actualQuantity = quantity,
                    maxQuantity = product.MaxQuantity
                });
            }
            return NotFound();
        }

        /// <summary>
        /// Возвращает общую стоимость товаров в корзине.
        /// </summary>
        /// <returns>JSON с общей стоимостью товаров.</returns>
        [HttpGet]
        public IActionResult GetTotal()
        {
            var total = _cartItemService.CartItems.Sum(x => x.Price * x.Quantity);
            return Json(new { total });
        }
    }
}
