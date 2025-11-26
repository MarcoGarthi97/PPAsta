using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.Helpers
{
    public class SrvInitializeYearNow
    {
        public bool IsYearInitialized { get; set; } = false;
        public int Year { get; set; } = DateTime.Now.Year;
    }
}
