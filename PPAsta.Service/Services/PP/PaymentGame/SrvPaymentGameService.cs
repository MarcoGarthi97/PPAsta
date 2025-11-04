using AutoMapper;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.PaymentGame;
using PPAsta.Repository.Services.Repositories.PP.PaymentGame;
using PPAsta.Service.Models.PaymentGame;
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
        Task<IEnumerable<SrvPaymentGame>> GetAllPaymentGamesAsync();
        Task InsertCardPaymentGameAsync(SrvPaymentGame paymentGame);
        Task InsertPaymentGameAsync(SrvPaymentGame paymentGame);
        Task InsertPaymentGamesAsync(IEnumerable<SrvPaymentGame> paymentGame);
        Task UpdatePaymentGameAsync(SrvPaymentGame paymentGame);
    }

    public class SrvPaymentGameService : ISrvPaymentGameService
    {
        private readonly IMapper _mapper;
        private readonly IMdlPaymentGameRepository _paymentGameRepository;
        private readonly ISrvPaymentService _paymentService;

        public SrvPaymentGameService(IMdlPaymentGameRepository paymentRepository, IMapper mapper, ISrvPaymentService paymentService)
        {
            _paymentGameRepository = paymentRepository;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        public async Task<IEnumerable<SrvPaymentGame>> GetAllPaymentGamesAsync()
        {
            var paymentsRepository = await _paymentGameRepository.GetAllPaymentGamesAsync();
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

        public async Task InsertCardPaymentGameAsync(SrvPaymentGame paymentGame)
        {
            MdlPaymentGame paymentGameDB = await _paymentGameRepository.GetPaymentGameAsyncByGameId(paymentGame.GameId);
            paymentGameDB.BuyerId = paymentGame.BuyerId;
            paymentGameDB.PaymentProcess = PaymentGameProcess.ToBePaid;
            paymentGameDB.SellingPrice = paymentGame.SellingPrice;
            paymentGameDB.PurchasePrice = paymentGame.PurchasePrice;
            paymentGameDB.ShareOwner = paymentGame.ShareOwner;
            paymentGameDB.SharePP = paymentGame.SharePP;
            paymentGameDB.RUD = DateTime.Now;

            int paymentId = await _paymentService.UpsertPaymentAsync(paymentGame);
            paymentGameDB.PaymentId = paymentId;

            await _paymentGameRepository.UpdatePaymentGameAsync(paymentGameDB);
        }
    }
}
