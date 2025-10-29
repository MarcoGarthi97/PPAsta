using PPAsta.Abstraction.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Payment
{
    public class SrvPaymentGame
    {        
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PaymentId { get; set; }
        public string Buyer { get; set; }
        public PaymentProcess PaymentProcess { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ShareOwner { get; set; }
        public decimal? SharePP { get; set; }
    }
}
