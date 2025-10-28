using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Models.Entities.Payment
{
    [Table("PAYMENT")]
    public class MdlPayment : MdlFieldsDB
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("PaymentID")]
        public int PaymentId { get; set; }
        [Column("Buyer")]
        public string? Buyer { get; set; }
        [Column("PaymentProcess")]
        public PayementProcess PayementProcess { get; set; }
        [Column("SellingPrice")]
        public decimal SellingPrice { get; set; }
        [Column("PurchasePrice")]
        public decimal? PurchasePrice { get; set; }
        [Column("ShareOwner")]
        public decimal? ShareOwner { get; set; }
        [Column("SharePP")]
        public decimal? SharePP { get; set; }
    }
}
