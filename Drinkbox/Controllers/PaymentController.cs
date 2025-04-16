using Drinkbox.Models;
using Drinkbox.Services.Coins;
using Microsoft.AspNetCore.Mvc;

namespace Drinkbox.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ICoinService _coinService;
        public PaymentController(ICoinService coinService)
        {
            _coinService = coinService;
        }
        public async Task<IActionResult> Index()
        {
            var coinList = await _coinService.GetAllAsync();
            return View(coinList);
        }
    }
}
