using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Game
{
    [Table("GAMES")]
    public class MdlGameDetail : MdlFieldsDB
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Owner")]
        public string Owner { get; set; }
        [Column("Year")]
        public int Year { get; set; }
        [Column("Buyer")]
        public string? Buyer { get; set; }
        [Column("PaymentProcess")]
        public PaymentGameProcess PaymentProcess { get; set; }
        [Column("PaymentType")]
        public PaymentType? PaymentType { get; set; }
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
