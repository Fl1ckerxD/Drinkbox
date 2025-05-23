﻿using Drinkbox.Core.DTOs;
using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;
using Drinkbox.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Drinkbox.Infrastructure.Services.CartItems
{
    /// <summary>
    /// Сервис для работы с элементами корзины.
    /// Реализует интерфейс ICartItemService для управления товарами в корзине.
    /// </summary>
    public class CartItemService : ICartItemService
    {
        private List<CartItem> _cartItems = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _uow;
        public List<CartItem> CartItems => _cartItems;
        public CartItemService(IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
        {
            _httpContextAccessor = httpContextAccessor;
            _uow = uow;
            LoadCart();
        }
        public void AddToCart(Product product, int quantity = 1)
        {
            if (product == null) throw new Exception("Не удалось добавить продукт в корзину - produсt is null");

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
            if (product == null) throw new Exception("Не удалось удалить продукт из корзины - produst is null");

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

            // Добавляем детали заказа для каждого товара в корзине
            foreach (var item in _cartItems)
            {
                var brand = await _uow.Brands.GetByIdAsync(item.BrandId);
                var orderItem = new OrderItem
                {
                    Order = order,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TotalPrice = item.Quantity * item.Price
                };

                _uow.Orders.AddOrderItem(orderItem);

                // Обновляем количество товара в базе данных
                var product = await _uow.Products.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity <= 0)
                        product.IsActive = false;
                }
            }

            await _uow.SaveAsync();

            _cartItems.Clear();
            SaveCart();
        }
    }
}
