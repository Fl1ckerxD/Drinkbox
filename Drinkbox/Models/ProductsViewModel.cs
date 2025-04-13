using Microsoft.AspNetCore.Mvc.Rendering;

namespace Drinkbox.Models
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; } = default!;
        public SelectList Brands { get; set; } = default!;
        public int? SelectedBrandID { get; set; }
    }
}
