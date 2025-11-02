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
        Task<IEnumerable<MdlBuyer>> GetAllBuyersAsync();
        Task<IEnumerable<MdlBuyer>> GetBuyerAsync(int number, int year);
        Task InsertBuyerAsync(MdlBuyer buyer);
        Task InsertBuyersAsync(IEnumerable<MdlBuyer> buyers);
        Task UpdateBuyerAsync(MdlBuyer buyer);
    }

    public class MdlBuyerRepository : BaseRepository<MdlBuyer>, IMdlBuyerRepository
    {
        public MdlBuyerRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<MdlBuyer>> GetBuyerAsync(int number, int year)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM BUYERS
                WHERE Number = @number AND Year = @year";

            return await connection.QueryAsync<MdlBuyer>(sql, new { number, year });
        }

        public async Task<IEnumerable<MdlBuyer>> GetAllBuyersAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlBuyer>();
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
    }
}
