using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Abstraction.Models.Entities
{
    public class MdlFieldsDB
    {
        [Column("RCD")]
        public DateTime RCD { get; set; }
        [Column("RUD")]
        public DateTime RUD { get; set; }
    }
}
