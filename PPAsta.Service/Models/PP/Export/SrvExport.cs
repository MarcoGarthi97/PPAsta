using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Export
{
    public class SrvExport
    {
        public string Articolo { get; set; }
        public string Venditore { get; set; }
        public decimal PrezzoPartenza { get; set; }
        public string Acquirente { get; set; }
        public bool Pagato { get; set; }
        public decimal PrezzoAcquisto { get; set; }
        public decimal QuotaPP { get; set; }
        public decimal QuataVenditore { get; set; }
    }
}
