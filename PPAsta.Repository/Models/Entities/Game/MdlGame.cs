using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPAsta.Repository.Models.Entities.Game
{
    [Table("GAMES")]
    public class MdlGame
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Owner")]
        public string Owner { get; set; }
        [Column("StartPrice")]
        public decimal StartPrice { get; set; }
        [Column("EndPrice")]
        public decimal? EndPrice { get; set; }
        [Column("Year")]
        public int Year { get; set; }
        [Column("IsSell")]
        public bool IsSell { get; set; }
        [Column("RCD")]
        public DateTime RCD { get; set; }
    }
}
