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
        Task DeletePaymentByBuyerIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<MdlPayment>> GetAllPaymentsAsync();
        Task<MdlPayment> GetPaymentByBuyerIdAsync(int buyerId);
        Task<MdlPayment> GetPaymentByIdAsync(int id);
        Task InsertPaymentAsync(MdlPayment Payment);
        Task InsertPaymentsAsync(IEnumerable<MdlPayment> Payments);
        Task UpdatePaymentAsync(MdlPayment Payment);
        Task<IEnumerable<MdlPaymentDetail>> GetAllPaymentDetailsAsync();
    }

    public class MdlPaymentRepository : BaseRepository<MdlPayment>, IMdlPaymentRepository
    {
        public MdlPaymentRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<MdlPayment> GetPaymentByBuyerIdAsync(int buyerId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM PAYMENTS WHERE BuyerID = @buyerId";

            return await connection.QueryFirstOrDefaultAsync<MdlPayment>(sql, new { buyerId });
        }

        public async Task<MdlPayment> GetPaymentByIdAsync(int buyerId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT P.* 
                FROM PAYMENTGAMES PG
                JOIN PAYMENTS P
                ON PG.BuyerID = P.BuyerID
                WHERE PG.BuyerID = @buyerId";

            return await connection.QueryFirstOrDefaultAsync<MdlPayment>(sql, new { buyerId });
        }

        public async Task<IEnumerable<MdlPayment>> GetAllPaymentsAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlPayment>();
        }

        public async Task<IEnumerable<MdlPaymentDetail>> GetAllPaymentDetailsAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = @"WITH CTE AS (
                SELECT BuyerID, G.Year, COUNT(*) AS TotalGames
                FROM PAYMENTGAMES P
                JOIN GAMES G ON P.GameID = G.ID
                WHERE BuyerID IS NOT NULL
                GROUP BY BuyerID, G.Year
                )

                SELECT P.*, B.Name as BuyerName, C.TotalGames, C.Year
                FROM PAYMENTS P
                JOIN BUYERS B ON P.BuyerID = B.ID
                JOIN CTE C ON C.BuyerID = B.ID";

            return await connection.QueryAsync<MdlPaymentDetail>(sql);
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

        public async Task DeletePaymentByBuyerIdsAsync(IEnumerable<int> ids)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = @"DELETE FROM PAYMENTS WHERE BuyerID IN @ids";

            await connection.QueryAsync(sql, new { ids });
        }
    }
}
