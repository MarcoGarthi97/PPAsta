using Dapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Helper;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PPAsta.Repository.Services.Repositories.PP.Helper
{
    public interface IMdlHelperRepository : IForServiceCollectionExtension
    {
        Task DeleteAllHelpersAsync();
        Task<MdlHelper> GetHelperByKeyAsync(string key);
        Task InsertHelpersAsync(IEnumerable<MdlHelper> helpers);
        Task UpdateHelperAsync(IEnumerable<MdlHelper> helpers);
        Task UpdateHelperByKeyAsync(MdlHelper helper);
    }
    public class MdlHelperRepository : BaseRepository<MdlHelper>, IMdlHelperRepository
    {
        public MdlHelperRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<MdlHelper> GetHelperByKeyAsync(string key)
        {
            string sql = "SELECT * FROM HELPERS WHERE Key = @key";

            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<MdlHelper>(sql, new
            {
                key
            });
        }

        public async Task InsertHelpersAsync(IEnumerable<MdlHelper> helpers)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(helpers);
        }

        public async Task UpdateHelperAsync(IEnumerable<MdlHelper> helpers)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            const string updateSql = @"
            UPDATE HELPERS 
            SET Json = @Json 
            WHERE Key = @Key";

            await connection.ExecuteAsync(updateSql, helpers);
        }

        public async Task UpdateHelperByKeyAsync(MdlHelper helper)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = @"
            UPDATE HELPERS 
            SET Json = @Json 
            WHERE Key = @Key";

            await connection.ExecuteAsync(sql, new { helper.Key, helper.Json });
        }

        public async Task DeleteAllHelpersAsync()
        {
            var sql = "DELETE FROM HELPERS";

            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.QueryAsync(sql);
        }
    }
}
