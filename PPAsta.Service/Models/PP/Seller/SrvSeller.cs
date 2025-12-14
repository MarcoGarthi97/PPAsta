using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;

namespace PPAsta.Service.Models.PP.Seller
{
    public class SrvSeller : SrvFields
    {
        public int Id { get; set; }
        public int PaymentGameId { get; set; }
        public PaymentSellerProcess PaymentSellerProcess { get; set; }
        public PaymentType? PaymentType { get; set; }
        public int Year { get; set; }
    }
}
