using System;
using System.Collections.Generic;

namespace Drinkbox.Models;

public partial class Coin
{
    public int CoinId { get; set; }

    public int Denomination { get; set; }

    public int Quantity { get; set; }
}
