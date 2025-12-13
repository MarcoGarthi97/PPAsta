using PPAsta.Abstraction.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Models.Entities.Export
{
    [Table("GAMES")]
    public class MdlExportCsv
    {
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
        [Column("PaymentSellerProcess")]
        public PaymentSellerProcess? PaymentSellerProcess { get; set; }
        [Column("PaymentTypeForSeller")]
        public PaymentType? PaymentTypeForSeller { get; set; }
    }
}
