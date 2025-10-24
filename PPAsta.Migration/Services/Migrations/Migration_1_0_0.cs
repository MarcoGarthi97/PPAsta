using Dapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Migration.Services.Migrations
{
    public interface IMigration_1_0_0 : IForServiceCollectionExtension
    {
        Task ExecuteMigration_1_0_0();
        string GetVersion();
    }

    public class Migration_1_0_0 : BaseRepositoryNonGeneric, IMigration_1_0_0
    {
        public Migration_1_0_0(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        public string GetVersion()
        {
            return "1.0.0";
        }

        public async Task ExecuteMigration_1_0_0()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.QueryAsync($"CREATE TABLE VERSION (" +
                $"Version VARCHAR(255) NOT NULL," +
                $"Rud DATETIME NOT NULL)");
        }
    }
}
