using System;
using System.Collections.Generic;

namespace Drinkbox.Models;

public partial class Coin
{
    public int CoinId { get; set; }
    public string Denomination { get; set; } = default!;
    public int? Value => int.Parse(Denomination.Split()[0]);
    public int Quantity { get; set; }
}
