using System;
using System.Collections.Generic;

namespace Drinkbox.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string BrandName { get; set; } = null!;

    public int Quantity { get; set; }

    public int UnitPrice { get; set; }

    public int TotalPrice { get; set; }

    public virtual Order Order { get; set; } = null!;
}
