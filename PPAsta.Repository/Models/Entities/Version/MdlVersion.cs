using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Models.Entities.Version
{
    [Table("VERSION")]
    public class MdlVersion
    {
        [Key]
        [Column("Version")]
        public string Version { get; set; }
        [Column("Rud")]
        public DateTime Rud { get; set; }
    }
}
