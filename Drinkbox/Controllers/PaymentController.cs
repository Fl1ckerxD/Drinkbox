using Drinkbox.Models;
using Drinkbox.Services.CartItems;
using Drinkbox.Services.Coins;
using Microsoft.AspNetCore.Mvc;

namespace Drinkbox.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var coinList = new List<Coin>(await _coinService.GetAllAsync());
            coinList.ForEach(c => c.Quantity = 0);
            return View(coinList);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] List<CoinInput> coinsInput)
        {
            try
            {
                var cartTotal = _cartItemService.CartItems.Sum(x => x.Price * x.Quantity);
                var paymentTotal = coinsInput.Sum(c => c.Value * c.Quantity);

                if (paymentTotal < cartTotal)
                    return Json(new { success = false, message = "Недостаточно средств" });

                await _coinService.SaveCoinsAsync(coinsInput);

                var change = paymentTotal - cartTotal;

                if (change > 0)
                {
                    var changeCoins = await _coinService.CalculateChange(change);
                    if (changeCoins != null)
                        await _coinService.UpdateQuantityCoins(changeCoins);
                }

                await _cartItemService.CompleteOrder();

                return Json(new
                {
                    success = true,
                    changeAmount = change,
                    //changeCoins = changeCoins
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки платежа");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
