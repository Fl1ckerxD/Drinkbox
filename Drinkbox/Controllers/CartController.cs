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
            var cartItems = _cartItemService.CartItems;
            return View(cartItems);
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

        [HttpGet]
        public IActionResult GetTotal()
        {
            var total = _cartItemService.CartItems.Sum(x => x.Price * x.Quantity);
            return Json(new { total });
        }
    }

    public record ToggleProductRequest(int productId, bool isSelected);
    //{
    //    public int productId { get; set; }
    //    public bool isSelected { get; set; }
    //}

    public record UpdateQuantityRequest(int productId, int quantity);
    //{
    //    public int productId { get; set; }
    //    public int quantity { get; set; }
    //}
}
