using Drinkbox.Services.CartItems;
using Drinkbox.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Drinkbox.Controllers
{
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productService.GetByIdAsync(productId);

            if (product == null) return NotFound();

            _cartItemService.AddToCart(product, quantity);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);

            if (product == null) return NotFound();

            _cartItemService.RemoveFromCart(product);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetCartStatus()
        {
            var cartItems = _cartItemService.CartItems;
            return Json(new { itemCount = cartItems.Count, hasItems = cartItems.Any() });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleProduct([FromBody] ToggleProductRequest request)
        {
            var product = await _productService.GetByIdAsync(request.productId);

            if (product == null) return NotFound();

            if (request.isSelected)
                _cartItemService.AddToCart(product);
            else
                _cartItemService.RemoveFromCart(product);

            return Ok();
        }
    }

    public class ToggleProductRequest
    {
        public int productId { get; set; }
        public bool isSelected { get; set; }
    }
}
