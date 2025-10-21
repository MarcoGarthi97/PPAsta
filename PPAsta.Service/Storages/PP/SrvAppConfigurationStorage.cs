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
