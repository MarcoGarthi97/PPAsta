using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Game
{
    public class SrvGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public decimal StartPrice { get; set; }
        public decimal? EndPrice { get; set; }
        public int Year { get; set; }
        public bool IsSell { get; set; }
        public DateTime RCD { get; set; }
    }
}
