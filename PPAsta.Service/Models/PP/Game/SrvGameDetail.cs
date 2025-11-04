using PPAsta.Abstraction.Models.Enums;

namespace PPAsta.Service.Models.PP.Game
{
    public class SrvGameDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Year { get; set; }
        public string Buyer { get; set; }
        public PaymentGameProcess PaymentProcess { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ShareOwner { get; set; }
        public decimal? SharePP { get; set; }
    }
}
