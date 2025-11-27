using PPAsta.Abstraction.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP.Buyer
{
    public class SrvBuyer : SrvFields
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }

        public SrvBuyer() { }

        public SrvBuyer(string name, int number, int year)
        {
            Name = name;
            Number = number;
            Year = year;
        }

        public SrvBuyer(int id, string name, int number, int year)
        {
            Id = id;
            Name = name;
            Number = number;
            Year = year;
        }
    }
}
