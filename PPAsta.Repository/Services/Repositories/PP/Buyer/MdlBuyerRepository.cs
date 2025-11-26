using Dapper;
using Dapper.Contrib.Extensions;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Buyer;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace PPAsta.Repository.Services.Repositories.PP.Buyer
{
    public interface IMdlBuyerRepository : IForServiceCollectionExtension
    {
        Task DeleteBuyerAsync(MdlBuyer buyer);
        Task<IEnumerable<MdlBuyer>> GetAllBuyersAsync(int? year = null);
        Task<MdlBuyer> GetBuyerByNumberAsync(int number, int year);
        Task<MdlBuyer> GetBuyerByIdAsync(int id);
        Task<MdlBuyer> GetBuyerByNameAsync(string name, int year);
        Task<int> GetNextNumberByYearAsync(int year);
        Task InsertBuyerAsync(MdlBuyer buyer);
        Task InsertBuyersAsync(IEnumerable<MdlBuyer> buyers);
        Task UpdateBuyerAsync(MdlBuyer buyer);
    }

    public class MdlBuyerRepository : BaseRepository<MdlBuyer>, IMdlBuyerRepository
    {
        public MdlBuyerRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<MdlBuyer> GetBuyerByNumberAsync(int number, int year)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM BUYERS
                WHERE Number = @number AND Year = @year";

            return await connection.QueryFirstOrDefaultAsync<MdlBuyer>(sql, new { number, year });
        }

        public async Task<MdlBuyer> GetBuyerByNameAsync(string name, int year)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM BUYERS
                WHERE Name = @name AND Year = @year";

            return await connection.QueryFirstOrDefaultAsync<MdlBuyer>(sql, new { name, year });
        }

        public async Task<MdlBuyer> GetBuyerByIdAsync(int id)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM BUYERS
                WHERE Id = @id";

            return await connection.QueryFirstOrDefaultAsync<MdlBuyer>(sql, new { id });
        }

        public async Task<IEnumerable<MdlBuyer>> GetAllBuyersAsync(int? year = null)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM BUYERS";

            if (year.HasValue)
            {
                sql += " WHERE YEAR = @year";
            }

            return await connection.QueryAsync<MdlBuyer>(sql, new { year });
        }

        public async Task InsertBuyerAsync(MdlBuyer buyer)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(buyer);
        }

        public async Task InsertBuyersAsync(IEnumerable<MdlBuyer> buyers)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(buyers);
        }

        public async Task UpdateBuyerAsync(MdlBuyer buyer)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.UpdateAsync(buyer);
        }

        public async Task DeleteBuyerAsync(MdlBuyer buyer)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.DeleteAsync(buyer);
        }

        public async Task<int> GetNextNumberByYearAsync(int year)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = @$"SELECT NUMBER + 1 FROM BUYERS 
                            WHERE Year = @year 
                            ORDER BY Number DESC
                            LIMIT 1;";

            return await connection.QueryFirstOrDefaultAsync<int>(sql, new { year });
        }
    }
}
