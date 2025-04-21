using Drinkbox.Models;
using Drinkbox.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Drinkbox.Services.CartItems
{
    /// <summary>
    /// Сервис для работы с элементами корзины.
    /// Реализует интерфейс ICartItemService для управления товарами в корзине.
    /// </summary>
    public class CartItemService : ICartItemService
    {
        private List<CartItem> _cartItems = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VendomatContext _context;
        public List<CartItem> CartItems => _cartItems;
        public CartItemService(IHttpContextAccessor httpContextAccessor, VendomatContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            LoadCart();
        }
        public void AddToCart(Product product, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = Math.Min(existingItem.Quantity + quantity, product.Quantity);
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = Math.Min(quantity, product.Quantity),
                    ImageUrl = product.ImageUrl,
                    MaxQuantity = product.Quantity,
                    BrandId = product.BrandId
                });
            }

            SaveCart();
        }

        public void SaveCart()
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

        public async Task CompleteOrder()
        {
            var totalPrice = _cartItems.Sum(x => x.Price * x.Quantity);

            var order = new Order
            {
                TotalSum = totalPrice
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Добавляем детали заказа для каждого товара в корзине
            foreach (var item in _cartItems)
            {
                var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == item.BrandId);
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    //ProductName = item.ProductName,
                    //BrandName = brand.BrandName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TotalPrice = totalPrice
                };
                _context.OrderItems.Add(orderItem);

                // Обновляем количество товара в базе данных
                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity <= 0)
                        product.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();

            _cartItems.Clear();
            SaveCart();
        }
    }
}
