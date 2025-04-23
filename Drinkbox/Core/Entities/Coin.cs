using System.ComponentModel.DataAnnotations;

namespace Drinkbox.Core.Entities
{
    public class Coin
    {
        public int CoinId { get; set; }

        [MaxLength(12)]
        public string Denomination { get; set; } = default!;
        public int? Value => int.Parse(Denomination.Split()[0]);
        public int Quantity { get; set; }
    }
}
