using AutoMapper;
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
        Task DeletePaymentAsync(SrvPayment Payment);
        Task<IEnumerable<SrvPayment>> GetAllPaymentsAsync();
        Task InsertPaymentAsync(SrvPayment Payment);
        Task InsertPaymentsAsync(IEnumerable<SrvPayment> Payment);
        Task UpdatePaymentAsync(SrvPayment Payment);
        Task<int> UpsertPaymentAsync(SrvPaymentGame paymentGame);
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

        public async Task<int> UpsertPaymentAsync(SrvPaymentGame paymentGame)
        {
            MdlPayment paymentRepository = await _paymentRepository.GetPaymentGameAsyncById(paymentGame.Id);

            int id;
            if (paymentRepository != null && paymentRepository.BuyerId != paymentGame.BuyerId)
            {
                var paymentGames = await _paymentGameService.GetAllPaymentGamesByBuyerIdAsync(paymentRepository.BuyerId);
                if (paymentGames.Count() == 1)
                {
                    await _paymentRepository.DeletePaymentAsync(paymentRepository);
                    paymentRepository = null;
                }
                else
                {
                    paymentRepository.TotalPurchasePrice -= paymentGame.PurchasePrice.Value;
                    paymentRepository.TotalShareOwner -= paymentGame.ShareOwner.Value;
                    paymentRepository.TotalSharePP -= paymentGame.SharePP.Value;
                    paymentRepository.RUD = DateTime.Now;

                    await _paymentRepository.UpdatePaymentAsync(paymentRepository);

                    paymentRepository = await _paymentRepository.GetPaymentGameAsyncByBuyerId(paymentGame.BuyerId.Value);
                }
            }

            if (paymentRepository == null)
            {
                SrvPayment paymentTemp = _mapper.Map<SrvPayment>(paymentGame);
                await InsertPaymentAsync(paymentTemp);

                paymentRepository = await _paymentRepository.GetPaymentGameAsyncByBuyerId(paymentGame.BuyerId.Value);
                id = paymentRepository.Id;
            }
            else
            {
                paymentRepository.BuyerId = paymentGame.BuyerId.Value;
                paymentRepository.TotalPurchasePrice += paymentGame.PurchasePrice.Value;
                paymentRepository.TotalShareOwner += paymentGame.ShareOwner.Value;
                paymentRepository.TotalSharePP += paymentGame.SharePP.Value;
                paymentRepository.RUD = DateTime.Now;

                await _paymentRepository.UpdatePaymentAsync(paymentRepository);

                id = paymentRepository.Id;
            }

            return id;
        }
    }
}
