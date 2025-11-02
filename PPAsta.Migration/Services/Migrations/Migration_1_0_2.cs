using Dapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Services.Store;

namespace PPAsta.Migration.Services.Migrations
{
    public interface IMigration_1_0_2 : IForServiceCollectionExtension
    {
        Task ExecuteMigration_1_0_2();
        string GetVersion();
    }

    public class Migration_1_0_2 : BaseRepositoryNonGeneric, IMigration_1_0_2
    {
        public Migration_1_0_2(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        public string GetVersion()
        {
            return "1.0.2";
        }

        public async Task ExecuteMigration_1_0_2()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.QueryAsync($@"CREATE TABLE BUYERS (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name VARCHAR NOT NULL,
                Number INTEGER NOT NULL,
                Year INTEGER NOT NULL,
                RCD DATETIME NOT NULL,
                RUD DATETIME NOT NULL
                )
            ");
        }
    }
}
