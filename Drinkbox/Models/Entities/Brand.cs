using System.ComponentModel.DataAnnotations;

namespace Drinkbox.Models.Entities
{
    public class Brand
    {
        public int BrandId { get; set; }

        [MaxLength(50)]
        public string BrandName { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
