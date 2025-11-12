using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Game;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace PPAsta.Repository.Services.Repositories.PP.Game
{
    public interface IMdlGameRepository : IForServiceCollectionExtension
    {
        Task DeleteGameAsync(MdlGame game);
        Task DeleteGameByYearsAsync(IEnumerable<int> years);
        Task<IEnumerable<MdlGame>> GetGamesByYearsAsync(IEnumerable<int> years);
        Task<IEnumerable<MdlGameDetail>> GetAllGameDetailsAsync();
        Task<IEnumerable<MdlGame>> GetAllGamesAsync();
        Task InsertGameAsync(MdlGame game);
        Task InsertGamesAsync(IEnumerable<MdlGame> games);
        Task UpdateGameAsync(MdlGame game);
    }

    public class MdlGameRepository : BaseRepository<MdlGame>, IMdlGameRepository
    {
        public MdlGameRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<MdlGame>> GetAllGamesAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlGame>();
        }

        public async Task InsertGameAsync(MdlGame game)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(game);
        }

        public async Task InsertGamesAsync(IEnumerable<MdlGame> games)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(games);
        }

        public async Task UpdateGameAsync(MdlGame game)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.UpdateAsync(game);
        }

        public async Task DeleteGameAsync(MdlGame game)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.DeleteAsync(game);
        }

        public async Task<IEnumerable<MdlGameDetail>> GetAllGameDetailsAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = @$"
                SELECT S.*, P.*, B.Name AS Buyer  
                FROM GAMES S
                JOIN PAYMENTGAMES P ON S.ID = P.GameID 
                LEFT JOIN BUYERS B ON B.ID = P.BuyerID 
            ";

            return await connection.QueryAsync<MdlGameDetail>(sql);
        }

        public async Task DeleteGameByYearsAsync(IEnumerable<int> years)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = "DELETE FROM GAMES WHERE Year in @years;";

            await connection.QueryAsync(sql, new { years });
        }

        public async Task<IEnumerable<MdlGame>> GetGamesByYearsAsync(IEnumerable<int> years)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();

            string sql = "SELECT * FROM GAMES WHERE Year in @years;";

            return await connection.QueryAsync<MdlGame>(sql, new { years });
        }
    }
}
