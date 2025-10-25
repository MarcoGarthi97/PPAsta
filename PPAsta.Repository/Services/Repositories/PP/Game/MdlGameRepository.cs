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
    }
}
