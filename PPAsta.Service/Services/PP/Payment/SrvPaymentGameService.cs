using AutoMapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Repository.Services.Repositories.PP.Payment;
using PPAsta.Service.Models.PP.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Payment
{    
    public interface ISrvPaymentService : IForServiceCollectionExtension
    {
        Task DeletePaymentAsync(SrvPaymentGame paymentGame);
        Task DeletePaymentGamesByGameIdsAsync(IEnumerable<int> gameIds);
        Task<IEnumerable<SrvPaymentGame>> GetAllPaymentsAsync();
        Task InsertPaymentAsync(SrvPaymentGame paymentGame);
        Task InsertPaymentsAsync(IEnumerable<SrvPaymentGame> paymentGame);
        Task UpdatePaymentAsync(SrvPaymentGame paymentGame);
    }

    public class SrvPaymentGameService : ISrvPaymentService
    {
        private readonly IMapper _mapper;
        private readonly IMdlPaymentRepository _paymentRepository;

        public SrvPaymentGameService(IMdlPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SrvPaymentGame>> GetAllPaymentsAsync()
        {
            var paymentsRepository = await _paymentRepository.GetAllPaymentsAsync();
            return _mapper.Map<IEnumerable<SrvPaymentGame>>(paymentsRepository);
        }

        public async Task InsertPaymentAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = _mapper.Map<MdlPaymentGame>(paymentGame);
            paymentRepository.RCD = DateTime.Now;
            paymentRepository.RUD = DateTime.Now;

            await _paymentRepository.InsertPaymentAsync(paymentRepository);
        }

        public async Task InsertPaymentsAsync(IEnumerable<SrvPaymentGame> paymentGame)
        {
            var paymentsRepository = _mapper.Map<IEnumerable<MdlPaymentGame>>(paymentGame);
            foreach (var paymentRepository in paymentsRepository)
            {
                paymentRepository.RCD = DateTime.Now;
                paymentRepository.RUD = DateTime.Now;
            }

            await _paymentRepository.InsertPaymentsAsync(paymentsRepository);
        }

        public async Task UpdatePaymentAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = _mapper.Map<MdlPaymentGame>(paymentGame);
            paymentRepository.RUD = DateTime.Now;

            await _paymentRepository.UpdatePaymentAsync(paymentRepository);
        }

        public async Task DeletePaymentAsync(SrvPaymentGame paymentGame)
        {
            var paymentRepository = _mapper.Map<MdlPaymentGame>(paymentGame);
            await _paymentRepository.DeletePaymentAsync(paymentRepository);
        }

        public async Task DeletePaymentGamesByGameIdsAsync(IEnumerable<int> gameIds)
        {
            await _paymentRepository.DeletePaymentGamesByGameIdsAsync(gameIds);
        }
    }
}
