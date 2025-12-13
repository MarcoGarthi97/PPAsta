using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Seller
{
    [Table("SELLERS")]
    public class MdlSellerCheck : MdlFieldsDB
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PaymentGameID")]
        public int PaymentGameId { get; set; }
        [Column("PaymentSellerProcess")]
        public PaymentSellerProcess PaymentSellerProcess { get; set; }
        [Column("Owner")]
        public string Owner { get; set; }
        [Column("Year")]
        public int Year { get; set; }
    }
}
