using AutoMapper;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Repository.Services.Repositories.PP.Payment;
using PPAsta.Service.Models.PP.Payment;
using PPAsta.Service.Models.PP.PaymentGame;
using PPAsta.Service.Services.PP.PaymentGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Payment
{
    public interface ISrvPaymentService : IForServiceCollectionExtension
    {
        Task HandlePaymentAsync(int buyerId);
        Task DeletePaymentAsync(SrvPayment Payment);
        Task DeletePaymentByBuyerIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<SrvPaymentDetail>> GetAllPaymentDetailsAsync();
        Task<IEnumerable<SrvPayment>> GetAllPaymentsAsync();
        Task<SrvPayment> GetPaymentByBuyerIdAsync(int buyerId);
        Task InsertPaymentAsync(SrvPayment Payment);
        Task InsertPaymentAsync(SrvPaymentGame paymentGame);
        Task InsertPaymentsAsync(IEnumerable<SrvPayment> Payment);
        Task RemoveGameForPaymentAsync(int oldBuyerId);
        Task UpdatePaymentAsync(SrvPayment Payment);
    }

    public class SrvPaymentService : ISrvPaymentService
    {
        private readonly IMapper _mapper;
        private readonly IMdlPaymentRepository _paymentRepository;
        private readonly ISrvPaymentGameService _paymentGameService;

        public SrvPaymentService(IMdlPaymentRepository paymentRepository, IMapper mapper, ISrvPaymentGameService paymentGameService)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _paymentGameService = paymentGameService;
        }

        public async Task<IEnumerable<SrvPayment>> GetAllPaymentsAsync()
        {
            var paymentsRepository = await _paymentRepository.GetAllPaymentsAsync();
            return _mapper.Map<IEnumerable<SrvPayment>>(paymentsRepository);
        }

        public async Task<IEnumerable<SrvPaymentDetail>> GetAllPaymentDetailsAsync()
        {
            var paymentsRepository = await _paymentRepository.GetAllPaymentDetailsAsync();
            return _mapper.Map<IEnumerable<SrvPaymentDetail>>(paymentsRepository);
        }

        public async Task<SrvPayment> GetPaymentByBuyerIdAsync(int buyerId)
        {
            var paymentRepository = await _paymentRepository.GetPaymentByBuyerIdAsync(buyerId);
            return _mapper.Map<SrvPayment>(paymentRepository);
        }

        public async Task InsertPaymentAsync(SrvPayment Payment)
        {
            var paymentRepository = _mapper.Map<MdlPayment>(Payment);
            paymentRepository.RCD = DateTime.Now;
            paymentRepository.RUD = DateTime.Now;

            await _paymentRepository.InsertPaymentAsync(paymentRepository);
        }

        public async Task InsertPaymentsAsync(IEnumerable<SrvPayment> Payment)
        {
            var paymentsRepository = _mapper.Map<IEnumerable<MdlPayment>>(Payment);
            foreach (var paymentRepository in paymentsRepository)
            {
                paymentRepository.RCD = DateTime.Now;
                paymentRepository.RUD = DateTime.Now;
            }

            await _paymentRepository.InsertPaymentsAsync(paymentsRepository);
        }

        public async Task UpdatePaymentAsync(SrvPayment Payment)
        {
            var paymentRepository = _mapper.Map<MdlPayment>(Payment);
            paymentRepository.RUD = DateTime.Now;

            await _paymentRepository.UpdatePaymentAsync(paymentRepository);
        }

        public async Task DeletePaymentAsync(SrvPayment Payment)
        {
            var paymentRepository = _mapper.Map<MdlPayment>(Payment);
            await _paymentRepository.DeletePaymentAsync(paymentRepository);
        }

        public async Task DeletePaymentByBuyerIdsAsync(IEnumerable<int> ids)
        {
            await _paymentRepository.DeletePaymentByBuyerIdsAsync(ids);
        }

        public async Task RemoveGameForPaymentAsync(int oldBuyerId)
        {
            var paymentRepository = await _paymentRepository.GetPaymentByIdAsync(oldBuyerId);
            var paymentGames = await _paymentGameService.GetAllPaymentGamesByBuyerIdAsync(oldBuyerId);
            if (paymentGames.Count() == 0)
            {
                await _paymentRepository.DeletePaymentByBuyerIdsAsync([oldBuyerId]);
                paymentRepository = null;
            }
            else
            {
                await HandlePaymentAsync(oldBuyerId);
            }
        }

        public async Task InsertPaymentAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = await _paymentRepository.GetPaymentByBuyerIdAsync(paymentGame.BuyerId.Value);
            if (paymentRepository == null)
            {
                SrvPayment paymentTemp = _mapper.Map<SrvPayment>(paymentGame);
                await InsertPaymentAsync(paymentTemp);

                paymentRepository = await _paymentRepository.GetPaymentByBuyerIdAsync(paymentGame.BuyerId.Value);
            }
            else
            {
                await HandlePaymentAsync(paymentGame.BuyerId.Value);
            }
        }

        public async Task HandlePaymentAsync(int buyerId)
        {
            var paymentGames = await _paymentGameService.GetAllPaymentGamesByBuyerIdAsync(buyerId);
            if (paymentGames.Any())
            {
                var paymentRepository = await _paymentRepository.GetPaymentByBuyerIdAsync(buyerId);
                if (paymentRepository != null)
                {
                    paymentRepository = CalculatedPayments(paymentRepository, paymentGames);
                    paymentRepository.PaymentProcess = GetPaymentProcess(paymentGames);

                    paymentRepository.RUD = DateTime.Now;

                    await _paymentRepository.UpdatePaymentAsync(paymentRepository);
                }
            }               
        }

        private PaymentProcess GetPaymentProcess(IEnumerable<SrvPaymentGame> paymentGames)
        {
            var gamesNotPayed = paymentGames.Where(x => x.PaymentProcess == PaymentGameProcess.ToBePaid);

            if (gamesNotPayed.Any() && gamesNotPayed.Count() == paymentGames.Count())
            {
                return PaymentProcess.ToBePaid;
            }
            else if (gamesNotPayed.Any())
            {
                return PaymentProcess.NotFullyPaid;
            }
            else
            {
                return PaymentProcess.Paid;
            }
        }

        private MdlPayment CalculatedPayments(MdlPayment paymentRepository, IEnumerable<SrvPaymentGame> paymentGames)
        {
            paymentRepository.TotalPurchasePrice = 0;
            paymentRepository.TotalShareOwner = 0;
            paymentRepository.TotalSharePP = 0;

            foreach (var paymentGame in paymentGames)
            {
                paymentRepository.TotalPurchasePrice += paymentGame.PurchasePrice.Value;
                paymentRepository.TotalShareOwner += paymentGame.ShareOwner.Value;
                paymentRepository.TotalSharePP += paymentGame.SharePP.Value;
            }

            return paymentRepository;
        }
    }
}
