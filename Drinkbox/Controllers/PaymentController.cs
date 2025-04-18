using Drinkbox.Models;
using Drinkbox.Services.CartItems;
using Drinkbox.Services.Coins;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Drinkbox.Controllers
{
    /// <summary>
    /// Контроллер для обработки платежей.
    /// </summary>
    public class PaymentController : Controller
    {
        private readonly ICoinService _coinService;
        private readonly ICartItemService _cartItemService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(ICoinService coinService, ICartItemService cartItemService,
            ILogger<PaymentController> logger)
        {
            _coinService = coinService;
            _cartItemService = cartItemService;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает страницу оплаты с информацией о доступных монетах.
        /// Если корзина пуста, перенаправляет на главную страницу.
        /// </summary>
        /// <returns>Представление оплаты или перенаправление на главную страницу.</returns>
        public async Task<IActionResult> Index()
        {
            if (_cartItemService.CartItems.Count == 0)
                return RedirectToAction("Index", "Home");

            var coinList = new List<Coin>(await _coinService.GetAllAsync());
            coinList.ForEach(c => c.Quantity = 0);
            return View(coinList);
        }

        /// <summary>
        /// Обрабатывает платеж, проверяет достаточность средств и выдает сдачу.
        /// </summary>
        /// <param name="coinsInput">Список монет, предоставленных пользователем.</param>
        /// <returns>JSON с результатом операции: успешность, сообщение об ошибке или URL для перенаправления.</returns>
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] List<CoinInput> coinsInput)
        {
            try
            {
                var cartTotal = _cartItemService.CartItems.Sum(x => x.Price * x.Quantity);
                var paymentTotal = coinsInput.Sum(c => c.Value * c.Quantity);

                if (paymentTotal < cartTotal)
                    return Json(new { success = false, message = "Недостаточно средств" });

                // Сохранение предоставленных монет в базу данных.
                await _coinService.SaveCoinsAsync(coinsInput);

                // Рассчитывание сдачи.
                var change = paymentTotal - cartTotal;
                Dictionary<int, int> changeCoins = new();

                // Если сдача больше нуля, рассчитываем её и обновляем количество монет.
                if (change > 0)
                {
                    changeCoins = await _coinService.CalculateChange(change);
                    if (changeCoins != null)
                        await _coinService.UpdateQuantityCoins(changeCoins);
                }

                // Завершаем заказ, очищая корзину.
                await _cartItemService.CompleteOrder();

                TempData["ChangeCoins"] = JsonSerializer.Serialize(changeCoins);
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("CompletePurchase", "Payment")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки платежа");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Отображает страницу завершения покупки с информацией о сдаче.
        /// </summary>
        /// <returns>Представление с информацией о сдаче или перенаправление на главную страницу.</returns>
        public IActionResult CompletePurchase()
        {
            if (TempData["ChangeCoins"] is string serializedCoins)
            {
                var changeCoins = JsonSerializer.Deserialize<Dictionary<int, int>>(serializedCoins);

                var denominations = new[] { 1, 2, 5, 10 };
                var change = new Dictionary<int, int>();
                foreach (var denom in denominations)
                {
                    if (changeCoins.ContainsKey(denom))
                        change.Add(denom, changeCoins[denom]);
                    else
                        change.Add(denom, 0);
                }
                return View(change);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
