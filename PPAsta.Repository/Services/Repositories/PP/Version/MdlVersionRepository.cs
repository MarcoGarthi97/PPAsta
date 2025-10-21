using Dapper;
using Microsoft.Data.Sqlite;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Version;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Services.Repositories.PP.Version
{
    public interface IMdlVersionRepository : IForServiceCollectionExtension
    {
        Task CreateTableVersionAsync();
        Task<string> GetVersionAsync();
        Task InsertVersionAsync(string version);
    }

    public class MdlVersionRepository : BaseRepository<MdlVersion>, IMdlVersionRepository
    {
        public MdlVersionRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task CreateTableVersionAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.QueryAsync($"CREATE TABLE VERSION (" +
                $"Version VARCHAR(255) NOT NULL," +
                $"Rud DATETIME NOT NULL)");
        }

        public async Task<string> GetVersionAsync()
        {
            try
            {
                var connection = await _connectionFactory.CreateConnectionAsync();
                return await connection.QueryFirstOrDefaultAsync<string>($"SELECT Version FROM Version");
            }
            catch (SqliteException ex)
            {
                return string.Empty;
            }
        }

        public async Task InsertVersionAsync(string version)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync($"INSERT INTO VERSION (Version, Rud) VALUES (@version, @rud)", new
            {
                version = version,
                rud = DateTime.Now
            });
        }
    }
}
