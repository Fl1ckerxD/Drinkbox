using System;
using System.Collections.Generic;

namespace Drinkbox.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int BrandId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Price { get; set; }

    public int Quantity { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Brand Brand { get; set; } = null!;
}
