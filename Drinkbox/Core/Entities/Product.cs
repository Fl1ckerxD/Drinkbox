using System.ComponentModel.DataAnnotations;

namespace Drinkbox.Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        [MaxLength(100)]
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
