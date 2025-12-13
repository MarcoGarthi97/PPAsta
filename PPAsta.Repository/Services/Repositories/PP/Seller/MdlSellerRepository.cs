using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.UI.Xaml;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Seller;
using PPAsta.Repository.Services.FactorySQL;
using PPAsta.Repository.Services.Repositories.PP.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Services.Repositories.PP.Seller
{
    public interface IMdlSellerRepository : IForServiceCollectionExtension
    {
        Task DeleteSellerByPayementGameIdAsync(int payementGameId);
        Task<IEnumerable<MdlSellerCheck>> GetAllSellersCheckAsync();
        Task<IEnumerable<MdlSellerDetail>> GetAllSellersDetailAsync();
        Task<MdlSellerDetail> GetSellerByPayementGameIdAsync(int payementGameId);
        Task<IEnumerable<MdlSeller>> GetSellerByPayementGameIdsAsync(IEnumerable<int> payementGameIds);
        Task InsertSeller(MdlSeller seller);
        Task UpdateSellersAsync(IEnumerable<MdlSeller> sellers);
    }

    public class MdlSellerRepository : BaseRepository<MdlSeller>, IMdlSellerRepository
    {
        public MdlSellerRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<MdlSellerDetail>> GetAllSellersDetailAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT
                G.Owner,
                G.Year,
                SUM(P.ShareOwner) AS TotalShareOwner,
                SUM(P.SharePP) AS TotalSharePP,
                COUNT(*) AS TotalGamesSold,
                (
                    SELECT COUNT(*)
                    FROM GAMES G2
                    WHERE G2.Owner = G.Owner
                ) AS TotalGames
            FROM GAMES G
            JOIN PAYMENTGAMES P
                ON G.ID = P.GameID
            JOIN SELLERS S
                ON S.PaymentGameID = P.ID
            GROUP BY
                G.Owner,
                G.Year;";

            return await connection.QueryAsync<MdlSellerDetail>(sql);
        }

        public async Task<IEnumerable<MdlSellerCheck>> GetAllSellersCheckAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"
	        SELECT S.*, G.Owner
	        FROM GAMES G
            JOIN PAYMENTGAMES P
            ON G.ID = P.GameID
            JOIN SELLERS S
            ON S.PaymentGameID = P.ID";

            return await connection.QueryAsync<MdlSellerCheck>(sql);
        }

        public async Task<MdlSellerDetail> GetSellerByPayementGameIdAsync(int payementGameId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM SELLERS WHERE PayementGameID = @payementGameId";

            return await connection.QueryFirstAsync<MdlSellerDetail>(sql, new { payementGameId });
        }

        public async Task<IEnumerable<MdlSeller>> GetSellerByPayementGameIdsAsync(IEnumerable<int> payementGameIds)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"SELECT * FROM SELLERS WHERE PaymentGameID in @payementGameIds";

            return await connection.QueryAsync<MdlSeller>(sql, new { payementGameIds });
        }

        public async Task InsertSeller(MdlSeller seller)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(seller);
        }

        public async Task UpdateSellersAsync(IEnumerable<MdlSeller> sellers)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkUpdateAsync(sellers);
        }

        public async Task DeleteSellerByPayementGameIdAsync(int payementGameId)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = $@"DELETE FROM SELLERS WHERE PayementGameID = @payementGameId";

            await connection.ExecuteAsync(sql, new { payementGameId });
        }
    }
}
