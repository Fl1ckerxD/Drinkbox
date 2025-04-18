using Drinkbox.Models;

namespace Drinkbox.Services.Coins
{
    public interface ICoinService
    {
        /// <summary>
        /// Асинхронно получает все доступные монеты из базы данных.
        /// </summary>
        /// <returns>Коллекция всех монет.</returns>
        Task<ICollection<Coin>> GetAllAsync();

        /// <summary>
        /// Сохраняет предоставленные пользователем монеты в базу данных.
        /// </summary>
        /// <param name="userCoins">Список монет, предоставленных пользователем.</param>
        Task SaveCoinsAsync(List<CoinInput> coins);

        /// <summary>
        /// Рассчитывает сдачу на основе доступных монет и требуемой суммы.
        /// </summary>
        /// <param name="changeAmount">Сумма сдачи, которую нужно выдать.</param>
        /// <returns>Словарь с номиналами монет и их количеством для сдачи.</returns>
        Task<Dictionary<int, int>> CalculateChange(int changeAmount);

        /// <summary>
        /// Обновляет количество монет в базе данных после выдачи сдачи.
        /// </summary>
        /// <param name="coinsToRemove">Словарь с номиналами монет и их количеством для удаления.</param>
        Task UpdateQuantityCoins(Dictionary<int, int> coinsToRemove);
    }
}
