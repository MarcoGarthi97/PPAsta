using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Models.Entities.Seller
{
    [Table("SELLERS")]
    public class MdlSellerDetail
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("Owner")]
        public string Owner { get; set; }
        [Column("TotalShareOwner")]
        public decimal TotalShareOwner { get; set; }
        [Column("TotalSharePP")]
        public decimal TotalSharePP { get; set; }
        [Column("TotalGames")]
        public int TotalGames { get; set; }
        [Column("TotalGamesSold")]
        public int TotalGamesSold { get; set; }
        [Column("Year")]
        public int Year { get; set; }
    }
}
