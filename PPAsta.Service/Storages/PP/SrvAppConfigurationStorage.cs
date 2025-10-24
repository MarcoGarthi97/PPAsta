using PPAsta.Service.Models.PP;
using PPAsta.Service.Models.PP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Storages.PP
{
    public static class SrvAppConfigurationStorage
    {
        public static SrvDatabaseConfiguration DatabaseConfiguration { get; private set; }
        public static SrvGoogleSpreadsheetConfiguration GoogleSpreadsheetConfiguration { get; private set; }

        public static void SetGoogleSpreadsheetConfiguration(string url)
        {
            // TODO: finire 
            string sheetID = "1nSSKkhASvRuoBdfUj_ztC9AM572qVZZ9lVvm5KUELIo";
            url = $"https://docs.google.com/spreadsheets/d/{sheetID}/export?format=csv";
            GoogleSpreadsheetConfiguration = new SrvGoogleSpreadsheetConfiguration(url);
        }

        public static void SetDatabasePath(string path)
        {
            CreateSrvDatabaseConfiguration();
            DatabaseConfiguration.Path = path;
        }

        private static void CreateSrvDatabaseConfiguration()
        {
            if (DatabaseConfiguration == null)
            {
                DatabaseConfiguration = new SrvDatabaseConfiguration();
            }
        }
    }
}
