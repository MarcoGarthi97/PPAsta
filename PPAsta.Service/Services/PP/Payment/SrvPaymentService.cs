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
        Task DeletePaymentAsync(SrvPayment Payment);
        Task<IEnumerable<SrvPayment>> GetAllPaymentsAsync();
        Task InsertPaymentAsync(SrvPayment Payment);
        Task InsertPaymentsAsync(IEnumerable<SrvPayment> Payments);
        Task UpdatePaymentAsync(SrvPayment Payment);
    }

    public class SrvPaymentService : ISrvPaymentService
    {
        private readonly IMapper _mapper;
        private readonly IMdlPaymentRepository _paymentRepository;

        public SrvPaymentService(IMdlPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SrvPayment>> GetAllPaymentsAsync()
        {
            var PaymentsRepository = await _paymentRepository.GetAllPaymentsAsync();
            return _mapper.Map<IEnumerable<SrvPayment>>(PaymentsRepository);
        }

        public async Task InsertPaymentAsync(SrvPayment Payment)
        {
            var paymentRepository = _mapper.Map<MdlPayment>(Payment);
            paymentRepository.RCD = DateTime.Now;
            paymentRepository.RUD = DateTime.Now;

            await _paymentRepository.InsertPaymentAsync(paymentRepository);
        }

        public async Task InsertPaymentsAsync(IEnumerable<SrvPayment> Payments)
        {
            var PaymentsRepository = _mapper.Map<IEnumerable<MdlPayment>>(Payments);
            foreach (var paymentRepository in PaymentsRepository)
            {
                paymentRepository.RCD = DateTime.Now;
                paymentRepository.RUD = DateTime.Now;
            }

            await _paymentRepository.InsertPaymentsAsync(PaymentsRepository);
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
    }
}
