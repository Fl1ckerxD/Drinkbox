using Drinkbox.Infrastructure.Services.CartItems;
using Drinkbox.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Drinkbox.Web.Components
{
    public class ImportViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new ImportExcelViewModel());
        }
    }
}
