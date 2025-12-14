using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Seller
{
    [Table("SELLERS")]
    public class MdlSeller : MdlFieldsDB
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PaymentGameID")]
        public int PaymentGameId { get; set; }
        [Column("PaymentSellerProcess")]
        public PaymentSellerProcess PaymentSellerProcess { get; set; }
        [Column("PaymentType")]
        public PaymentType? PaymentType { get; set; }
        [Column("Year")]
        public int Year { get; set; }
    }
}
