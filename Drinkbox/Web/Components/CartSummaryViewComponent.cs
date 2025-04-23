using Drinkbox.Infrastructure.Services.CartItems;
using Microsoft.AspNetCore.Mvc;

namespace Drinkbox.Web.Components
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ICartItemService _cartItemService;
        public CartSummaryViewComponent(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            _cartItemService.LoadCart();
            var count = _cartItemService.CartItems.Count;
            return View(count);
        }
    }
}
