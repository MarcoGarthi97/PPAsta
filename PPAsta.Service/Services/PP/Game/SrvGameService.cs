using AutoMapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Game;
using PPAsta.Repository.Services.FactorySQL;
using PPAsta.Repository.Services.Repositories.PP.Game;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Game
{
    public interface ISrvGameService : IForServiceCollectionExtension
    {
        Task DeleteGameAsync(SrvGame game);
        Task DeleteGameByYearsAsync(IEnumerable<int> years);
        Task<IEnumerable<SrvGameDetail>> GetAllGameDetailsAsync();
        Task<IEnumerable<SrvGameDetail>> GetAllGameDetailsByBuyerIdAsync(int buyerId);
        Task<IEnumerable<SrvGame>> GetAllGamesAsync();
        Task<IEnumerable<SrvGame>> GetGamesByYearsAsync(IEnumerable<int> years);
        Task<int?> GetOldestYearAsync();
        Task InsertGameAsync(SrvGame game);
        Task InsertGamesAsync(IEnumerable<SrvGame> games);
        Task UpdateGameAsync(SrvGame game);
    }

    public class SrvGameService : ISrvGameService
    {
        private readonly IMapper _mapper;
        private readonly IMdlGameRepository _gameRepository;

        public SrvGameService(IMdlGameRepository gameRepository, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SrvGame>> GetAllGamesAsync()
        {
            var gamesRepository = await _gameRepository.GetAllGamesAsync();
            return _mapper.Map<IEnumerable<SrvGame>>(gamesRepository);
        }

        public async Task<IEnumerable<SrvGameDetail>> GetAllGameDetailsAsync()
        {
            var gamesRepository = await _gameRepository.GetAllGameDetailsAsync();
            return _mapper.Map<IEnumerable<SrvGameDetail>>(gamesRepository);
        }

        public async Task<IEnumerable<SrvGameDetail>> GetAllGameDetailsByBuyerIdAsync(int buyerId)
        {
            var gamesRepository = await _gameRepository.GetAllGameDetailsByBuyerIdAsync(buyerId);
            return _mapper.Map<IEnumerable<SrvGameDetail>>(gamesRepository);
        }

        public async Task InsertGameAsync(SrvGame game)
        {
            var gameRepository = _mapper.Map<MdlGame>(game);
            gameRepository.RCD = DateTime.Now;
            gameRepository.RUD = DateTime.Now;

            await _gameRepository.InsertGameAsync(gameRepository);
        }

        public async Task InsertGamesAsync(IEnumerable<SrvGame> games)
        {
            var oldestGames = games.Where(x => x.Year < SrvAppConfigurationStorage.OldestYear).OrderByDescending(x => x.Year);
            if (oldestGames.Any())
            {
                SrvAppConfigurationStorage.SetOldestYear(oldestGames.FirstOrDefault().Year);
            }

            var gamesRepository = _mapper.Map<IEnumerable<MdlGame>>(games);
            foreach (var gameRepository in gamesRepository)
            {
                gameRepository.RCD = DateTime.Now;
                gameRepository.RUD = DateTime.Now;
            }

            await _gameRepository.InsertGamesAsync(gamesRepository);
        }

        public async Task UpdateGameAsync(SrvGame game)
        {
            var gameRepository = _mapper.Map<MdlGame>(game);
            gameRepository.RUD = DateTime.Now;

            await _gameRepository.UpdateGameAsync(gameRepository);
        }

        public async Task DeleteGameAsync(SrvGame game)
        {
            var gameRepository = _mapper.Map<MdlGame>(game);
            await _gameRepository.DeleteGameAsync(gameRepository);
        }

        public async Task<IEnumerable<SrvGame>> GetGamesByYearsAsync(IEnumerable<int> years)
        {
            var gamesRepository = await _gameRepository.GetGamesByYearsAsync(years);

            return _mapper.Map<IEnumerable<SrvGame>>(gamesRepository);
        }

        public async Task DeleteGameByYearsAsync(IEnumerable<int> years)
        {
            await _gameRepository.DeleteGameByYearsAsync(years);
        }

        public async Task<int?> GetOldestYearAsync()
        {
            try
            {
                return await _gameRepository.GetOldestYearAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
