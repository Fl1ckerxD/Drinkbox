using System.ComponentModel.DataAnnotations;

namespace Drinkbox.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public int MaxQuantity { get; set; }
        public int BrandId { get; set; }
    }
}
