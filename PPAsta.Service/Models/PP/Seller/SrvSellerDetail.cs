using PPAsta.Abstraction.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Seller
{
    public class SrvSellerDetail
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public decimal TotalShareOwner { get; set; }
        public decimal TotalSharePP { get; set; }
        public int TotalGames { get; set; }
        public int TotalGamesSold { get; set; }
        public int Year { get; set; }
        public PaymentSellerProcess PaymentSellerProcess { get; set; }
    }
}
