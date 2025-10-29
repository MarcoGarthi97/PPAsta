using AutoMapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Game;
using PPAsta.Repository.Services.FactorySQL;
using PPAsta.Repository.Services.Repositories.PP.Game;
using PPAsta.Service.Models.PP.Game;
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
        Task<IEnumerable<SrvGameDetail>> GetAllGameDetailsAsync();
        Task<IEnumerable<SrvGame>> GetAllGamesAsync();
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

        public async Task InsertGameAsync(SrvGame game)
        {
            var gameRepository = _mapper.Map<MdlGame>(game);
            gameRepository.RCD = DateTime.Now;
            gameRepository.RUD = DateTime.Now;

            await _gameRepository.InsertGameAsync(gameRepository);
        }

        public async Task InsertGamesAsync(IEnumerable<SrvGame> games)
        {
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
    }
}
