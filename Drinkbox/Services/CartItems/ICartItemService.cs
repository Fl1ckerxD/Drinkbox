using Drinkbox.Models;
using Drinkbox.Models.Entities;

namespace Drinkbox.Services.CartItems
{
    public interface ICartItemService
    {
        /// <summary>
        /// Текущий список товаров в корзине.
        /// </summary>
        List<CartItem> CartItems { get; }

        /// <summary>
        /// Добавляет товар в корзину или обновляет его количество, если он уже есть.
        /// </summary>
        /// <param name="product">Товар для добавления в корзину.</param>
        /// <param name="quantity">Количество товара (по умолчанию 1).</param>
        void AddToCart(Product product, int quantity = 1);

        /// <summary>
        /// Удаляет товар из корзины.
        /// </summary>
        /// <param name="product">Товар для удаления из корзины.</param>
        void RemoveFromCart(Product product);

        /// <summary>
        /// Загружает состояние корзины из сессии.
        /// </summary>
        void LoadCart();

        /// <summary>
        /// Сохраняет текущее состояние корзины в сессии.
        /// </summary>
        void SaveCart();

        /// <summary>
        /// Завершает оформление заказа, создавая запись в базе данных и обновляя количество товаров.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task CompleteOrder();
    }
}
