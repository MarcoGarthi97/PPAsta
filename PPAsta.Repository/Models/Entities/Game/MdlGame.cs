using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Game
{
    [Table("GAMES")]
    public class MdlGame : MdlFieldsDB
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Owner")]
        public string Owner { get; set; }
        [Column("Year")]
        public int Year { get; set; }
    }
}
