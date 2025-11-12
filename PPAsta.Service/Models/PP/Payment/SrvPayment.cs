using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;

namespace PPAsta.Service.Models.PP.Payment
{
    public class SrvPayment : SrvFields
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public PaymentProcess PaymentProcess { get; set; }
        public decimal TotalPurchasePrice { get; set; }
        public decimal TotalShareOwner { get; set; }
        public decimal TotalSharePP { get; set; }
    }
}
