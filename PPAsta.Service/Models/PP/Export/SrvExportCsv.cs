using PPAsta.Abstraction.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Export
{
    public class SrvExportCsv
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Year { get; set; }
        public string Buyer { get; set; }
        public PaymentGameProcess PaymentProcess { get; set; }
        public PaymentType? PaymentType { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ShareOwner { get; set; }
        public decimal? SharePP { get; set; }
        public PaymentSellerProcess? PaymentSellerProcess { get; set; }
        public PaymentType? PaymentTypeForSeller { get; set; }
    }
}
