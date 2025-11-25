using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Abstraction.Models.Entities
{
    public class ComboBoxPP
    {
        public string DisplayName { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
