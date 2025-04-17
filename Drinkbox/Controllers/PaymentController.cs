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
        public PaymentController(ICoinService coinService, ICartItemService cartItemService)
        {
            _coinService = coinService;
            _cartItemService = cartItemService;
        }
        public async Task<IActionResult> Index()
        {
            var coinList = new List<Coin>(await _coinService.GetAllAsync());
            coinList.ForEach(c => c.Quantity = 0);
            return View(coinList);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] List<Coin> coinsInput)
        {
            var cartTotal = _cartItemService.CartItems.Sum(x => x.Price * x.Quantity);
            var paymentTotal = coinsInput.Sum(c => c.Value * c.Quantity);

            if (paymentTotal < cartTotal)
                return Json(new { success = false, message = "Недостаточно средств" });

            await _coinService.SaveCoinsAsync(coinsInput);

            var change = paymentTotal - cartTotal;
            var changeCoins = CalculateChange(change.Value);

            _cartItemService.ClearCart();

            return Json(new
            {
                success = true,
                changeAmount = change,
                changeCoins = changeCoins
            });
        }

        private Dictionary<int, int> CalculateChange(int changeAmount)
        {
            var denominations = new[] { 10, 5, 2, 1 };
            var change = new Dictionary<int, int>();

            foreach (var denom in denominations)
            {
                if (changeAmount >= denom)
                {
                    int count = changeAmount / denom;
                    change.Add(denom, count);
                    changeAmount -= count * denom;
                }
            }

            return change;
        }
    }

    
}
