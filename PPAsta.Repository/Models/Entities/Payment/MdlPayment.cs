using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Payment
{
    [Table("PAYMENTS")]
    public class MdlPayment : MdlFieldsDB
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("BuyerID")]
        public int BuyerId { get; set; }
        [Column("PaymentProcess")]
        public PaymentProcess PaymentProcess { get; set; }
        [Column("TotalPurchasePrice")]
        public decimal TotalPurchasePrice { get; set; }
        [Column("TotalShareOwner")]
        public decimal TotalShareOwner { get; set; }
        [Column("TotalSharePP")]
        public decimal TotalSharePP { get; set; }
    }
}
