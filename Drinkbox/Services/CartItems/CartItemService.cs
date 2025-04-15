using Drinkbox.Models;
using System.Text.Json;

namespace Drinkbox.Services.CartItems
{
    public class CartItemService : ICartItemService
    {
        private List<CartItem> _cartItems = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<CartItem> CartItems => _cartItems;
        public CartItemService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            LoadCart();
        }
        public void AddToCart(Product product, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl,
                });
            }

            SaveCart();
        }

        private void SaveCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("Cart", JsonSerializer.Serialize(_cartItems));
        }

        public void LoadCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            _cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        public void RemoveFromCart(Product product)
        {
            var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (existingItem == null)
                return;

            _cartItems.Remove(existingItem);
            SaveCart();
        }
    }
}
