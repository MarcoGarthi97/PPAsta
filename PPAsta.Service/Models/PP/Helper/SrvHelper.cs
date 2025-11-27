using PPAsta.Abstraction.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Helper
{
    public class SrvHelper : SrvFields
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Json { get; set; }
    }
}
