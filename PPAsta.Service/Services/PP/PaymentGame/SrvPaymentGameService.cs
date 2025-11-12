using AutoMapper;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.PaymentGame;
using PPAsta.Repository.Services.Repositories.PP.PaymentGame;
using PPAsta.Service.Models.PP.PaymentGame;
using PPAsta.Service.Services.PP.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.PaymentGame
{    
    public interface ISrvPaymentGameService : IForServiceCollectionExtension
    {
        Task DeletePaymentGameAsync(SrvPaymentGame paymentGame);
        Task DeletePaymentGamesByGameIdsAsync(IEnumerable<int> gameIds);
        Task<IEnumerable<SrvPaymentGame>> GetAllPaymentGamesAsync();
        Task<IEnumerable<SrvPaymentGame>> GetAllPaymentGamesByBuyerIdAsync(int buyerId);
        Task<SrvPaymentGame> GetPaymentGameAsyncByGameIdAsync(int gameId);
        Task<IEnumerable<SrvPaymentGame>> GetPaymentGameAsyncByGameIdsAsync(IEnumerable<int> gameIds);
        Task InsertPaymentGameAsync(SrvPaymentGame paymentGame);
        Task InsertPaymentGamesAsync(IEnumerable<SrvPaymentGame> paymentGame);
        Task UpdatePaymentGameAsync(SrvPaymentGame paymentGame);
    }

    public class SrvPaymentGameService : ISrvPaymentGameService
    {
        private readonly IMapper _mapper;
        private readonly IMdlPaymentGameRepository _paymentGameRepository;

        public SrvPaymentGameService(IMdlPaymentGameRepository paymentRepository, IMapper mapper)
        {
            _paymentGameRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SrvPaymentGame>> GetAllPaymentGamesAsync()
        {
            var paymentsRepository = await _paymentGameRepository.GetAllPaymentGamesAsync();
            return _mapper.Map<IEnumerable<SrvPaymentGame>>(paymentsRepository);
        }

        public async Task<IEnumerable<SrvPaymentGame>> GetAllPaymentGamesByBuyerIdAsync(int buyerId)
        {
            var paymentsRepository = await _paymentGameRepository.GetAllPaymentGamesBybuyerIdAsync(buyerId);
            return _mapper.Map<IEnumerable<SrvPaymentGame>>(paymentsRepository);
        }

        public async Task InsertPaymentGameAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = _mapper.Map<MdlPaymentGame>(paymentGame);
            paymentRepository.RCD = DateTime.Now;
            paymentRepository.RUD = DateTime.Now;

            await _paymentGameRepository.InsertPaymentGameAsync(paymentRepository);
        }

        public async Task InsertPaymentGamesAsync(IEnumerable<SrvPaymentGame> paymentGame)
        {
            var paymentsRepository = _mapper.Map<IEnumerable<MdlPaymentGame>>(paymentGame);
            foreach (var paymentRepository in paymentsRepository)
            {
                paymentRepository.RCD = DateTime.Now;
                paymentRepository.RUD = DateTime.Now;
            }

            await _paymentGameRepository.InsertPaymentGamesAsync(paymentsRepository);
        }

        public async Task UpdatePaymentGameAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = _mapper.Map<MdlPaymentGame>(paymentGame);
            paymentRepository.RUD = DateTime.Now;

            await _paymentGameRepository.UpdatePaymentGameAsync(paymentRepository);
        }

        public async Task DeletePaymentGameAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = _mapper.Map<MdlPaymentGame>(paymentGame);
            await _paymentGameRepository.DeletePaymentGameAsync(paymentRepository);
        }

        public async Task<SrvPaymentGame> GetPaymentGameAsyncByGameIdAsync(int gameId)
        {
            MdlPaymentGame paymentGameDB = await _paymentGameRepository.GetPaymentGameAsyncByGameIdAsync(gameId);
            return _mapper.Map<SrvPaymentGame>(paymentGameDB);
        }

        public async Task<IEnumerable<SrvPaymentGame>> GetPaymentGameAsyncByGameIdsAsync(IEnumerable<int> gameIds)
        {
            var paymentGamesDB = await _paymentGameRepository.GetPaymentGameAsyncByGameIdsAsync(gameIds);
            return _mapper.Map<IEnumerable<SrvPaymentGame>>(paymentGamesDB);
        }

        public async Task DeletePaymentGamesByGameIdsAsync(IEnumerable<int> gameIds)
        {
            await _paymentGameRepository.DeletePaymentGamesByGameIdsAsync(gameIds);
        }
    }
}
