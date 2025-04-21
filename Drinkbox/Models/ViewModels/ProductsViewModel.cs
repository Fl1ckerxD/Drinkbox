using Drinkbox.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Drinkbox.Models.ViewModels
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; } = default!;
        public SelectList Brands { get; set; } = default!;
        public HashSet<int> ProductsInCart { get; set; } = new();
    }
}
