using Dapper;
using Dapper.Contrib.Extensions;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Repository.Models.Entities.PaymentGame;
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
        Task DeletePaymentAsync(MdlPayment Payment);
        Task DeletePaymentByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<MdlPayment>> GetAllPaymentsAsync();
        Task<MdlPayment> GetPaymentGameAsyncByBuyerId(int buyerId);
        Task<MdlPayment> GetPaymentGameAsyncById(int id);
        Task InsertPaymentAsync(MdlPayment Payment);
        Task InsertPaymentsAsync(IEnumerable<MdlPayment> Payments);
        Task UpdatePaymentAsync(MdlPayment Payment);
    }

    public class MdlPaymentRepository : BaseRepository<MdlPayment>, IMdlPaymentRepository
    {
        public MdlPaymentRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<MdlPayment> GetPaymentGameAsyncByBuyerId(int buyerId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM PAYMENTS WHERE BuyerID = @buyerId";

            return await connection.QueryFirstOrDefaultAsync<MdlPayment>(sql, new { buyerId });
        }

        public async Task<MdlPayment> GetPaymentGameAsyncById(int gameId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT P.* 
                FROM PAYMENTGAMES PG
                JOIN PAYMENTS P
                ON PG.PaymentID = P.ID
                WHERE PG.ID = @gameId";

            return await connection.QueryFirstOrDefaultAsync<MdlPayment>(sql, new { gameId });
        }

        public async Task<IEnumerable<MdlPayment>> GetAllPaymentsAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlPayment>();
        }

        public async Task InsertPaymentAsync(MdlPayment Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(Payment);
        }

        public async Task InsertPaymentsAsync(IEnumerable<MdlPayment> Payments)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(Payments);
        }

        public async Task UpdatePaymentAsync(MdlPayment Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.UpdateAsync(Payment);
        }

        public async Task DeletePaymentAsync(MdlPayment Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.DeleteAsync(Payment);
        }

        public async Task DeletePaymentByIdsAsync(IEnumerable<int> ids)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = @"DELETE FROM PAYMENTS WHERE ID IN @ids";

            await connection.QueryAsync(sql, new { ids });
        }
    }
}
