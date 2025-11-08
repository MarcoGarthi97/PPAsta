using Dapper;
using Dapper.Contrib.Extensions;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace PPAsta.Repository.Services.Repositories.PP.Payment
{
    public interface IMdlPaymentRepository : IForServiceCollectionExtension
    {
        Task DeletePaymentAsync(MdlPaymentGame Payment);
        Task DeletePaymentGamesByGameIdsAsync(IEnumerable<int> gameIds);
        Task<IEnumerable<MdlPaymentGame>> GetAllPaymentsAsync();
        Task InsertPaymentAsync(MdlPaymentGame Payment);
        Task InsertPaymentsAsync(IEnumerable<MdlPaymentGame> Payments);
        Task UpdatePaymentAsync(MdlPaymentGame Payment);
    }

    public class MdlPaymentGameRepository : BaseRepository<MdlPaymentGame>, IMdlPaymentRepository
    {
        public MdlPaymentGameRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<MdlPaymentGame>> GetAllPaymentsAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlPaymentGame>();
        }

        public async Task InsertPaymentAsync(MdlPaymentGame Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(Payment);
        }

        public async Task InsertPaymentsAsync(IEnumerable<MdlPaymentGame> Payments)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(Payments);
        }

        public async Task UpdatePaymentAsync(MdlPaymentGame Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.UpdateAsync(Payment);
        }

        public async Task DeletePaymentAsync(MdlPaymentGame Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.DeleteAsync(Payment);
        }

        public async Task DeletePaymentGamesByGameIdsAsync(IEnumerable<int> gameIds)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"DELETE FROM PAYMENTGAMES WHERE GameID in @gameIds;";

            await connection.QueryAsync(sql, new { gameIds });
        }
    }
}
