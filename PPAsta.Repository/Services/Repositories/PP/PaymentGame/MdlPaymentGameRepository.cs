using Dapper;
using Dapper.Contrib.Extensions;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.PaymentGame;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace PPAsta.Repository.Services.Repositories.PP.PaymentGame
{
    public interface IMdlPaymentGameRepository : IForServiceCollectionExtension
    {
        Task DeletePaymentGameAsync(MdlPaymentGame Payment);
        Task<IEnumerable<MdlPaymentGame>> GetAllPaymentGamesAsync();
        Task<MdlPaymentGame> GetPaymentGameAsyncByGameId(int gameId);
        Task InsertPaymentGameAsync(MdlPaymentGame Payment);
        Task InsertPaymentGamesAsync(IEnumerable<MdlPaymentGame> Payments);
        Task UpdatePaymentGameAsync(MdlPaymentGame Payment);
    }

    public class MdlPaymentGameRepository : BaseRepository<MdlPaymentGame>, IMdlPaymentGameRepository
    {
        public MdlPaymentGameRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<MdlPaymentGame> GetPaymentGameAsyncByGameId(int gameId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM PAYMENTGAMES WHERE GameID = @gameId";

            return await connection.QueryFirstAsync<MdlPaymentGame>(sql, new { gameId });
        }

        public async Task<IEnumerable<MdlPaymentGame>> GetAllPaymentGamesAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlPaymentGame>();
        }

        public async Task InsertPaymentGameAsync(MdlPaymentGame Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(Payment);
        }

        public async Task InsertPaymentGamesAsync(IEnumerable<MdlPaymentGame> Payments)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(Payments);
        }

        public async Task UpdatePaymentGameAsync(MdlPaymentGame Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.UpdateAsync(Payment);
        }

        public async Task DeletePaymentGameAsync(MdlPaymentGame Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.DeleteAsync(Payment);
        }
    }
}
