using PPAsta.Service.Models.Helpers;
using PPAsta.Service.Models.PP;
using PPAsta.Service.Models.PP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PPAsta.Service.Storages.PP
{
    public static class SrvAppConfigurationStorage
    {
        public static SrvDatabaseConfiguration DatabaseConfiguration { get; private set; }
        public static SrvInitializeYearNow InitializeYearNow { get; private set; } = new SrvInitializeYearNow();
        public static int OldestYear { get; private set; } = DateTime.Now.Year;

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

        public static void SetDatabaseExist()
        {
            CreateSrvDatabaseConfiguration();
            DatabaseConfiguration.DatabaseExists = true;
        }

        public static void SetOldestYear(int? year)
        {
            if(year.HasValue && year.Value > 0)
            {
                OldestYear = year.Value;
            }
        }

        public static void SetInitializeYearNow(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                InitializeYearNow = JsonSerializer.Deserialize<SrvInitializeYearNow>(json);
            }
        }
    }
}
