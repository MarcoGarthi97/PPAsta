using PPAsta.Abstraction.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Buyer
{
    [Table("BUYERS")]
    public class MdlBuyer : MdlFieldsDB
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Number")]
        public int Number { get; set; }
        [Column("Year")]
        public int Year { get; set; }
    }
}
