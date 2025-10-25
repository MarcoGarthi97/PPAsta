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
    public interface IMigration_1_0_1 : IForServiceCollectionExtension
    {
        Task ExecuteMigration_1_0_1();
        string GetVersion();
    }

    public class Migration_1_0_1 : BaseRepositoryNonGeneric, IMigration_1_0_1
    {
        public Migration_1_0_1(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        public string GetVersion()
        {
            return "1.0.1";
        }

        public async Task ExecuteMigration_1_0_1()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.QueryAsync($@"CREATE TABLE HELPERS (
                ID INTEGER PRIMARY KEY,
                Key VARCHAR NOT NULL,
                Json VARCHAR NOT NULL
                )
            ");

            await connection.QueryAsync($@"CREATE TABLE GAMES (
                ID INTEGER PRIMARY KEY,
                Name VARCHAR NOT NULL,
                Owner VARCHAR NOT NULL,
                StartPrice DOUBLE NOT NULL,
                EndPrice DOUBLE NULL,
                Year REAL NOT NULL,
                RCD DATETIME NOT NULL
                )
            ");
        }
    }
}
