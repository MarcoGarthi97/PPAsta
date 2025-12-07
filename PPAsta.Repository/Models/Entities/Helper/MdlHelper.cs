using PPAsta.Abstraction.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Models.Entities.Helper
{
    [Table("HELPERS")]
    public class MdlHelper : MdlFieldsDB
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Key")]
        public string Key { get; set; }
        [Column("Json")]
        public string Json { get; set; }
    }
}
