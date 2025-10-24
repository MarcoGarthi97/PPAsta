using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Models.PP
{
    public class SrvGoogleSpreadsheetConfiguration
    {
        public string Url { get; set; }

        public SrvGoogleSpreadsheetConfiguration() { }

        public SrvGoogleSpreadsheetConfiguration(string url) { Url = url; }
    }
}
