﻿using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Services.Products
{
    public interface IProductService
    {
        /// <summary>
        /// Асинхронно получает продукт по его идентификатору.
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        /// <returns>Продукт с указанным идентификатором или null, если продукт не найден.</returns>
        Task<Product> GetByIdAsync(int productId);

        /// <summary>
        /// Асинхронно получает все доступные продукты из базы данных.
        /// </summary>
        /// <returns>Коллекция всех продуктов.</returns>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        /// Получает продукты, фильтруя их по идентификатору бренда.
        /// </summary>
        /// <param name="brandId">Идентификатор бренда (необязательный).</param>
        /// <returns>Коллекция продуктов, отфильтрованных по бренду.</returns>
        Task<IEnumerable<Product>> GetByBrandAsync(int? brandId);

        /// <summary>
        /// Фильтрует продукты по максимальной цене.
        /// </summary>
        /// <param name="products">Коллекция продуктов для фильтрации.</param>
        /// <param name="maxPrice">Максимальная цена для фильтрации.</param>
        /// <returns>Коллекция продуктов, цена которых не превышает указанную максимальную цену.</returns>
        ICollection<Product> GetByMaxPrice(IEnumerable<Product> products, int maxPrice);
    }
}
